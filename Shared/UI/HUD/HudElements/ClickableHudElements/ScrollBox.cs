using VRageMath;
using System;
using System.Collections.Generic;

namespace RichHudFramework.UI
{
    /// <summary>
    /// Scrollable list of hud elements. Can be oriented vertically or horizontally. Min/Max size determines
    /// the maximum size of scrollbox elements as well as the scrollbox itself.
    /// </summary>
    public sealed class ScrollBox<T> : HudChain<T> where T : HudElementBase
    {
        /// <summary>
        /// Parallel list indicating which chain members are/aren't enabled.
        /// </summary>
        public IReadOnlyList<bool> EnabledElements => enabledElements;

        /// <summary>
        /// Minimum number of visible elements allowed. Supercedes maximum size. If the number of elements that
        /// can fit within the maximum size is less than this value, then this element will expand beyond its maximum
        /// size.
        /// </summary>
        public int MinimumVisCount { get; set; }

        /// <summary>
        /// Index of the first element in the visible range in the chain.
        /// </summary>
        public int Start
        {
            get { return _start; }
            set 
            {
                _start = MathHelper.Clamp(value, 0, enabledElements.Count - 1);
                updateRangeReverse = false;
            }
        }

        /// <summary>
        /// Index of the last element in the visible range in the chain.
        /// </summary>
        public int End
        {
            get { return _end; } 
            set 
            {
                _end = MathHelper.Clamp(value, 0, enabledElements.Count - 1);
                updateRangeReverse = true;
            } 
        }

        /// <summary>
        /// Position of the first visible element as it appears in the UI
        /// </summary>
        public int VisStart { get; private set; }

        /// <summary>
        /// Number of elements visible starting from the Start index
        /// </summary>
        public int VisCount { get; private set; }

        /// <summary>
        /// Total number of enabled elements
        /// </summary>
        public int EnabledCount { get; private set; }

        /// <summary>
        /// Background color of the scroll box.
        /// </summary>
        public Color Color { get { return background.Color; } set { background.Color = value; } }

        public bool EnableScrolling 
        { 
            get { return scrollInput.Visible; } 
            set 
            { 
                scrollInput.Visible = value;
                scrollBar.slide.MouseInput.CaptureCursor = value;
            } 
        }

        private bool updateRangeReverse;
        private int _start, _end;

        public readonly ScrollBar scrollBar;
        public readonly TexturedBox background;
        public readonly TexturedBox divider;

        private readonly MouseInputFilter scrollInput;
        private readonly List<bool> enabledElements;

        public ScrollBox(bool alignVertical, IHudParent parent = null) : base(alignVertical, parent)
        {
            background = new TexturedBox(this)
            {
                ZOffset = HudLayers.Background,
                Color = new Color(41, 54, 62, 196),
                DimAlignment = DimAlignments.Both,
            };

            scrollInput = new MouseInputFilter(this)
            {
                Binds = new IBind[] { SharedBinds.MousewheelUp, SharedBinds.MousewheelDown },
                DimAlignment = DimAlignments.Both
            };

            scrollBar = new ScrollBar(this);
            divider = new TexturedBox(scrollBar) { Color = new Color(53, 66, 75) };

            MinimumVisCount = 1;
            enabledElements = new List<bool>();

            if (alignVertical)
            {
                scrollBar.DimAlignment = DimAlignments.Height;
                divider.DimAlignment = DimAlignments.Height;

                scrollBar.ParentAlignment = ParentAlignments.Right | ParentAlignments.InnerH;
                divider.ParentAlignment = ParentAlignments.Left | ParentAlignments.InnerH;

                divider.Padding = new Vector2(2f, 0f);
                divider.Width = 1f;

                scrollBar.Padding = new Vector2(30f, 8f);
                scrollBar.Width = 45f;
            }
            else
            {
                scrollBar.DimAlignment = DimAlignments.Width;
                divider.DimAlignment = DimAlignments.Width;

                scrollBar.ParentAlignment = ParentAlignments.Bottom | ParentAlignments.InnerV;
                divider.ParentAlignment = ParentAlignments.Bottom | ParentAlignments.InnerV;

                divider.Padding = new Vector2(16f, 2f);
                divider.Height = 1f;

                scrollBar.Padding = new Vector2(16f);
                scrollBar.Height = 24f;
            }
        }

        /// <summary>
        /// Adds a <see cref="T"/> to the scrollbox.
        /// </summary>
        public override void Add(T element)
        {
            base.Add(element);
            enabledElements.Add(true);
        }

        /// <summary>
        /// Adds a <see cref="T"/> to the scrollbox.
        /// </summary>
        public void Add(T element, bool enabled)
        {
            base.Add(element);
            enabledElements.Add(enabled);
        }

        /// <summary>
        /// Add the given range to the end of the scrollbox.
        /// </summary>
        public override void AddRange(IList<T> newChainElements)
        {
            base.AddRange(newChainElements);
            enabledElements.EnsureCapacity(enabledElements.Count + newChainElements.Count);

            for (int n = 0; n < enabledElements.Count; n++)
                enabledElements.Add(true);
        }

        /// <summary>
        /// Insert the given range into the chain.
        /// </summary>
        public override void InsertRange(int index, IList<T> newChainElements)
        {
            base.InsertRange(index, newChainElements);
            enabledElements.EnsureCapacity(enabledElements.Count + newChainElements.Count);

            for (int n = index; n < index + newChainElements.Count; n++)
                enabledElements.Insert(n, true);
        }

        /// <summary>
        /// Remove the scrollbox element at the given index.
        /// </summary>
        public override void RemoveAt(int index)
        {
            base.RemoveAt(index);
            enabledElements.RemoveAt(index);
        }

        /// <summary>
        /// Removes the specfied range from the chain. Normal child elements not affected.
        /// </summary>
        public override void RemoveRange(int index, int count)
        {
            base.RemoveRange(index, count);
            enabledElements.RemoveRange(index, count);
        }

        /// <summary>
        /// Remove all elements in the HudChain. Does not affect normal child elements.
        /// </summary>
        public override void ClearChain()
        {
            base.ClearChain();
            enabledElements.Clear();
        }

        protected override void RemoveChildInternal(object childID)
        {
            if (!blockChildRegistration)
            {
                int index = children.FindIndex(x => x.ID == childID);

                if (index != -1)
                {
                    if (children[index].Parent?.ID == ID)
                        children[index].Unregister();
                    else if (children[index].Parent == null)
                        children.RemoveAt(index);
                }
                else
                {
                    index = chainElements.FindIndex(x => x.ID == childID);

                    if (index != -1)
                    {
                        if (chainElements[index].Parent?.ID == ID)
                            chainElements[index].Unregister();
                        else if (chainElements[index].Parent == null)
                        {
                            chainElements.RemoveAt(index);
                            enabledElements.RemoveAt(index);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Used to enable/disable a scrollbox element.
        /// </summary>
        public void SetElementEnabled(int index, bool value)
        {
            enabledElements[index] = value;
        }

        protected override void HandleInput()
        {
            // Don't capture cursor if there's no where to scroll to
            scrollInput.CaptureCursor = scrollBar.Min != scrollBar.Max;

            if (scrollBar.MouseInput.IsLeftClicked)
                _start = (int)Math.Round(scrollBar.Current);

            if (scrollInput.IsControlPressed)
            {
                if (SharedBinds.MousewheelUp.IsPressed)
                    _start = MathHelper.Clamp(_start - 1, 0, enabledElements.Count - 1);
                else if (SharedBinds.MousewheelDown.IsPressed)
                    _start = MathHelper.Clamp(_start + 1, 0, enabledElements.Count - 1);
            }

            scrollBar.Current = _start;
        }

        protected override void Layout()
        {
            // Calculate effective min and max element sizes
            Vector2 effectivePadding = cachedPadding;
            float scrollBarPadding = scrollBar.Size[offAxis] + divider.Size[offAxis];
            effectivePadding[offAxis] += scrollBarPadding;

            Vector2 largest = GetLargestElementSize(),
                elementMin = MinimumSize - effectivePadding,
                elementMax = MaximumSize - effectivePadding;

            ClampElementSizeRange(largest, ref elementMin, ref elementMax);

            Vector2 newSize = GetNewSize(elementMax, largest);
            scrollBar.Current = (int)Math.Round(scrollBar.Current);

            // Update visible range
            UpdateElementRange(elementMax[alignAxis]);
            UpdateElementVisibility();

            // Update scrollbox and slider size

            // Visible total size will be greater than total size if its not large enough
            // to meet minimum vis count
            newSize = Vector2.Max(newSize, GetVisibleTotalSize());

            // scrollbar range
            scrollBar.Min = GetFirstEnabled();
            scrollBar.Max = GetScrollMax(newSize[alignAxis]);

            Vector2 enabledTotalSize = GetEnabledTotalSize();
            UpdateSliderSize(newSize[alignAxis] / enabledTotalSize[alignAxis]);

            cachedSize = newSize;
            cachedSize[offAxis] += scrollBarPadding;
            _absoluteWidth = cachedSize.X / _scale;
            _absoluteHeight = cachedSize.Y / _scale;
            cachedSize += cachedPadding;

            // Calculate member start offset
            Vector2 startOffset;

            if (alignAxis == 1)
                startOffset = new Vector2(-scrollBarPadding, newSize.Y) * .5f;
            else
                startOffset = new Vector2(-newSize.X, scrollBarPadding) * .5f;

            UpdateMemberOffsets(startOffset, effectivePadding, elementMin, elementMax);
        }

        /// <summary>
        /// Returns the index of the first enabled element in the list.
        /// </summary>
        private int GetFirstEnabled()
        {
            for (int n = 0; n < enabledElements.Count; n++)
            {
                if (enabledElements[n])
                    return n;
            }

            return 0;
        }

        /// <summary>
        /// Calculates the maximum index offset for the scroll bar
        /// </summary>
        private int GetScrollMax(float length)
        {
            int start = 0, visCount = 0;

            // Find new ending index
            for (int n = chainElements.Count - 1; n >= 0; n--)
            {
                if (enabledElements[n])
                {
                    if (length >= chainElements[n].Size[alignAxis] || VisCount < MinimumVisCount)
                    {
                        start = n;
                        length -= chainElements[n].Size[alignAxis];
                        visCount++;
                    }
                    else
                        break;

                    length -= Spacing;
                }
            }

            return start;
        }

        /// <summary>
        /// Updates the range of visible members starting with the given start index.
        /// If the starting index doesn't satisfy the minimum visible count, it will 
        /// be decreased until it does.
        /// </summary>
        private void UpdateElementRange(float length)
        {
            VisCount = 0;
            EnabledCount = GetVisibleIndex(chainElements.Count);

            if (updateRangeReverse)
                UpdateElementRangeReverse(length);
            else
                UpdateElementRangeForward(length);
            
            VisStart = GetVisibleIndex(_start);
        }

        /// <summary>
        /// Updates range of visible elements starting with the starting index.
        /// </summary>
        private void UpdateElementRangeForward(float length)
        {
            Vector2I range = new Vector2I(_start);

            for (int n = _start; n < chainElements.Count; n++)
            {
                if (enabledElements[n])
                {
                    if (length >= chainElements[n].Size[alignAxis] || VisCount < MinimumVisCount)
                    {
                        range.Y = n;
                        length -= chainElements[n].Size[alignAxis];
                        VisCount++;
                    }
                    else
                        break;

                    length -= Spacing;
                }
            }

            if (EnabledCount > VisCount)
            {
                // Move starting index back until minimum visible requirment is met
                for (int n = _start - 1; (n >= 0 && VisCount < MinimumVisCount); n--)
                {
                    if (enabledElements[n])
                    {
                        range.X = n;
                        VisCount++;
                    }
                }
            }

            _start = range.X;
            _end = range.Y;
        }

        /// <summary>
        /// Updates range of visible elements starting with the ending index.
        /// </summary>
        private void UpdateElementRangeReverse(float length)
        {
            Vector2I range = new Vector2I(_end);

            for (int n = _end; n >= 0; n--)
            {
                if (enabledElements[n])
                {
                    if (length >= chainElements[n].Size[alignAxis] || VisCount < MinimumVisCount)
                    {
                        range.X = n;
                        length -= chainElements[n].Size[alignAxis];
                        VisCount++;
                    }
                    else
                        break;

                    length -= Spacing;
                }
            }

            if (EnabledCount > VisCount)
            {
                // Move starting index back until minimum visible requirment is met
                for (int n = _end + 1; (n < chainElements.Count && VisCount < MinimumVisCount); n++)
                {
                    if (enabledElements[n])
                    {
                        range.Y = n;
                        VisCount++;
                    }
                }
            }

            _start = range.X;
            _end = range.Y;
        }

        /// <summary>
        /// Sets the visibility of all elements in the scroll box to false.
        /// </summary>
        private void UpdateElementVisibility()
        {
            if (chainElements.Count > 0)
            {
                for (int n = 0; n < chainElements.Count; n++)
                    chainElements[n].Visible = false;

                for (int n = _start; n <= _end; n++)
                    chainElements[n].Visible = enabledElements[n];
            }
        }

        /// <summary>
        /// Returns the number of enabled elements before the one at the given index
        /// </summary>
        private int GetVisibleIndex(int index)
        {
            int count = 0;

            for (int n = 0; n < index; n++)
            {
                if (enabledElements[n])
                    count++;
            }

            return count;
        }

        /// <summary>
        /// Returns the total size of the enabled elements in the chain.
        /// </summary>
        private Vector2 GetEnabledTotalSize()
        {
            Vector2 newSize = new Vector2();

            for (int n = 0; n < chainElements.Count; n++)
            {
                if (enabledElements[n])
                {
                    Vector2 elementSize = chainElements[n].Size;

                    // Total up the size of elements on the axis of alignment
                    newSize[alignAxis] += elementSize[alignAxis];

                    // Find largest element on the off axis
                    if (elementSize[offAxis] > newSize[offAxis])
                        newSize[offAxis] = elementSize[offAxis];

                    newSize[alignAxis] += Spacing;
                }
            }

            newSize[alignAxis] -= Spacing;
            return Vector2.Max(newSize, Vector2.Zero);
        }

        /// <summary>
        /// Returns the size of the largest element in the chain.
        /// </summary>
        private Vector2 GetLargestElementSize()
        {
            Vector2 size = new Vector2();

            for (int n = 0; n < chainElements.Count; n++)
            {
                if (enabledElements[n])
                    size = Vector2.Max(size, chainElements[n].Size);
            }

            return size;
        }

        /// <summary>
        /// Recalculates and updates the height of the scroll bar.
        /// </summary>
        private void UpdateSliderSize(float visRatio)
        {
            Vector2 sliderSize = scrollBar.slide.SliderSize;
            sliderSize[alignAxis] = visRatio * cachedSize[alignAxis];
            scrollBar.slide.SliderSize = sliderSize;
        }
    }
}
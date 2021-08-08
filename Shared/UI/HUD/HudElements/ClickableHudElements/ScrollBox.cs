using VRageMath;
using VRage;
using System;
using System.Collections.Generic;
using ApiMemberAccessor = System.Func<object, int, object>;

namespace RichHudFramework.UI
{
    using HudUpdateAccessors = MyTuple<
        ApiMemberAccessor,
        MyTuple<Func<ushort>, Func<Vector3D>>, // ZOffset + GetOrigin
        Action, // DepthTest
        Action, // HandleInput
        Action<bool>, // BeforeLayout
        Action // BeforeDraw
    >;

    /// <summary>
    /// Scrollable list of hud elements. Can be oriented vertically or horizontally. Min/Max size determines
    /// the maximum size of scrollbox elements as well as the scrollbox itself.
    /// </summary>
    public class ScrollBox<TElementContainer, TElement> : HudChain<TElementContainer, TElement>
        where TElementContainer : IScrollBoxEntry<TElement>, new()
        where TElement : HudElementBase
    {
        /// <summary>
        /// Width of the scrollbox
        /// </summary>
        public override float Width
        {
            set
            {
                if (value > Padding.X)
                    value -= Padding.X;

                float scale = (LocalScale * parentScale);
                _absoluteWidth = value / scale;

                if (offAxis == 0)
                {
                    if (value > 0f && (SizingMode & (HudChainSizingModes.ClampMembersOffAxis | HudChainSizingModes.FitMembersOffAxis)) > 0)
                        _absMaxSize.X = (value - scrollBarPadding) / scale;
                }
                else
                    _absMinLengthInternal = _absoluteWidth;
            }
        }

        /// <summary>
        /// Height of the scrollbox
        /// </summary>
        public override float Height
        {
            set
            {
                if (value > Padding.Y)
                    value -= Padding.Y;

                float scale = (LocalScale * parentScale);
                _absoluteHeight = value / scale;

                if (offAxis == 1)
                {
                    if (value > 0f && (SizingMode & (HudChainSizingModes.ClampMembersOffAxis | HudChainSizingModes.FitMembersOffAxis)) > 0)
                        _absMaxSize.Y = (value - scrollBarPadding) / scale;
                }
                else
                    _absMinLengthInternal = _absoluteHeight;
            }
        }

        /// <summary>
        /// Minimum number of visible elements allowed. Supercedes maximum length. If the number of elements that
        /// can fit within the maximum length is less than this value, then this element will expand beyond its maximum
        /// size.
        /// </summary>
        public int MinVisibleCount { get; set; }

        /// <summary>
        /// Minimum total length (on the align axis) of visible members allowed in the scrollbox.
        /// </summary>
        public float MinLength { get { return _absMinLength * (LocalScale * parentScale); } set { _absMinLength = value / (LocalScale * parentScale); } }

        /// <summary>
        /// Index of the first element in the visible range in the chain.
        /// </summary>
        public int Start
        {
            get { return MathHelper.Clamp(_start, 0, hudCollectionList.Count - 1); }
            set
            {
                if (value != _start)
                {
                    _start = MathHelper.Clamp(value, 0, hudCollectionList.Count - 1);
                    updateRangeReverse = false;
                    ScrollBar.Current = GetMinScrollOffset(_start, false);
                }
            }
        }

        /// <summary>
        /// Index of the last element in the visible range in the chain.
        /// </summary>
        public int End
        {
            get { return MathHelper.Clamp(_end, 0, hudCollectionList.Count - 1); }
            set
            {
                if (value != _end)
                {
                    _end = MathHelper.Clamp(value, 0, hudCollectionList.Count - 1);
                    updateRangeReverse = true;
                    ScrollBar.Current = GetMinScrollOffset(_end, true);
                }
            }
        }

        /// <summary>
        /// Position of the first visible element as it appears in the UI. Does not correspond to actual index.
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
        public Color Color { get { return Background.Color; } set { Background.Color = value; } }

        /// <summary>
        /// Color of the slider bar
        /// </summary>
        public Color BarColor { get { return ScrollBar.slide.BarColor; } set { ScrollBar.slide.BarColor = value; } }

        /// <summary>
        /// Bar color when moused over
        /// </summary>
        public Color BarHighlight { get { return ScrollBar.slide.BarHighlight; } set { ScrollBar.slide.BarHighlight = value; } }

        /// <summary>
        /// Color of the slider box when not moused over
        /// </summary>
        public Color SliderColor { get { return ScrollBar.slide.SliderColor; } set { ScrollBar.slide.SliderColor = value; } }

        /// <summary>
        /// Color of the slider button when moused over
        /// </summary>
        public Color SliderHighlight { get { return ScrollBar.slide.SliderHighlight; } set { ScrollBar.slide.SliderHighlight = value; } }

        public bool EnableScrolling { get; set; }

        public override bool AlignVertical
        {
            set
            {
                ScrollBar.Vertical = value;
                base.AlignVertical = value;

                if (value)
                {
                    ScrollBar.DimAlignment = DimAlignments.Height;
                    Divider.DimAlignment = DimAlignments.Height;

                    ScrollBar.ParentAlignment = ParentAlignments.Right | ParentAlignments.InnerH;
                    Divider.ParentAlignment = ParentAlignments.Left | ParentAlignments.InnerH;

                    Divider.Padding = new Vector2(2f, 0f);
                    Divider.Width = 1f;

                    ScrollBar.Padding = new Vector2(30f, 10f);
                    ScrollBar.Width = 43f;
                }
                else
                {
                    ScrollBar.DimAlignment = DimAlignments.Width;
                    Divider.DimAlignment = DimAlignments.Width;

                    ScrollBar.ParentAlignment = ParentAlignments.Bottom | ParentAlignments.InnerV;
                    Divider.ParentAlignment = ParentAlignments.Bottom | ParentAlignments.InnerV;

                    Divider.Padding = new Vector2(16f, 2f);
                    Divider.Height = 1f;

                    ScrollBar.Padding = new Vector2(16f);
                    ScrollBar.Height = 24f;
                }
            }
        }

        public ScrollBar ScrollBar { get; protected set; }
        public TexturedBox Divider { get; protected set; }
        public TexturedBox Background { get; protected set; }

        protected float scrollBarPadding, _absMinLength, _absMinLengthInternal;
        protected bool updateRangeReverse;
        protected int _start, _end, firstEnabled;

        public ScrollBox(bool alignVertical, HudParentBase parent = null) : base(alignVertical, parent)
        {
            Background = new TexturedBox(this)
            {
                Color = TerminalFormatting.DarkSlateGrey,
                DimAlignment = DimAlignments.Both,
                ZOffset = -1,
            };

            MinVisibleCount = 1;
            UseCursor = true;
            ShareCursor = false;
            EnableScrolling = true;
            ZOffset = 1;
        }

        protected override void Init()
        {
            ScrollBar = new ScrollBar(this);
            Divider = new TexturedBox(ScrollBar) { Color = new Color(53, 66, 75) };
        }

        public ScrollBox(HudParentBase parent) : this(false, parent)
        { }

        public ScrollBox() : this(false, null)
        { }

        protected override void HandleInput(Vector2 cursorPos)
        {
            if (hudCollectionList.Count > 0 && EnableScrolling && (IsMousedOver || ScrollBar.IsMousedOver))
            {
                if (SharedBinds.MousewheelUp.IsPressed)
                {
                    ScrollBar.Current -= hudCollectionList[_end].Element.Size[alignAxis] + Spacing;
                }
                else if (SharedBinds.MousewheelDown.IsPressed)
                {
                    ScrollBar.Current += hudCollectionList[_start].Element.Size[alignAxis] + Spacing;
                }
            }
        }

        protected override void Layout()
        {
            // Calculate effective min and max element sizes
            Vector2 effectivePadding = cachedPadding;
            scrollBarPadding = ScrollBar.Size[offAxis];
            effectivePadding[offAxis] += scrollBarPadding;

            UpdateMemberSizes();

            // Get the list length
            float scale = (LocalScale * parentScale),
                rangeLength = Math.Max(MinLength, _absMinLengthInternal * scale);

            Vector2 size = cachedSize,
                visibleTotalSize = GetVisibleTotalSize(),
                listSize = size - effectivePadding,
                scrollOffset = Vector2.Zero;

            size = listSize;
            size[offAxis] += scrollBarPadding;
            _absoluteWidth = size.X / scale;
            _absoluteHeight = size.Y / scale;

            // Update scrollbar max bound and calculate offset for scrolling
            ScrollBar.Max = Math.Max(visibleTotalSize[alignAxis] - listSize[alignAxis], 0f);
            scrollOffset[alignAxis] = ScrollBar.Current;
            firstEnabled = 0;

            for (int i = 0; i < hudCollectionList.Count; i++)
            {
                TElementContainer entry = hudCollectionList[i];

                if (entry.Enabled)
                {
                    if (firstEnabled == 0)
                        firstEnabled = i;

                    float memberLength = entry.Element.Size[alignAxis] + Spacing;

                    if (scrollOffset[alignAxis] <= memberLength)
                    {
                        _start = i;
                        break;
                    }
                    else
                        scrollOffset[alignAxis] -= memberLength;
                }
            }

            // Update visible range
            UpdateElementRange(rangeLength + scrollOffset[alignAxis]);

            // Update slider size
            float visRatio = Math.Max(listSize[alignAxis] / visibleTotalSize[alignAxis], 0f);
            Vector2 sliderSize = ScrollBar.slide.BarSize;

            sliderSize[alignAxis] = (Size[alignAxis] - ScrollBar.Padding[alignAxis]) * visRatio;
            ScrollBar.slide.SliderSize = sliderSize;

            // Calculate member start offset
            Vector2 startOffset;

            if (alignAxis == 1)
                startOffset = new Vector2(-scrollBarPadding * .5f, listSize.Y * .5f + scrollOffset[alignAxis]);
            else
                startOffset = new Vector2(-listSize.X * .5f - scrollOffset[alignAxis], scrollBarPadding * .5f);

            UpdateMemberOffsets(startOffset, effectivePadding);
        }

        /// <summary>
        /// Updates the range of visible members starting with the given start index.
        /// If the starting index doesn't satisfy the minimum visible count, it will 
        /// be decreased until it does.
        /// </summary>
        private void UpdateElementRange(float length)
        {
            EnabledCount = GetVisibleIndex(hudCollectionList.Count);
            _start = MathHelper.Clamp(_start, firstEnabled, hudCollectionList.Count);

            if (updateRangeReverse)
                UpdateElementRangeReverse(length);
            else
                UpdateElementRangeForward(length);

            updateRangeReverse = false;
            VisStart = GetVisibleIndex(_start);

            for (int n = 0; n < hudCollectionList.Count; n++)
                hudCollectionList[n].Element.Visible = (n >= _start && n <= _end) && hudCollectionList[n].Enabled;
        }

        /// <summary>
        /// Updates range of visible elements starting with the starting index.
        /// </summary>
        private void UpdateElementRangeForward(float length)
        {
            Vector2I range = new Vector2I(_start);
            VisCount = 0;

            for (int n = _start; n < hudCollectionList.Count; n++)
            {
                if (hudCollectionList[n].Enabled)
                {
                    TElement element = hudCollectionList[n].Element;

                    if (length > 0f || VisCount < MinVisibleCount)
                    {
                        range.Y = n;
                        length -= element.Size[alignAxis];
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
                for (int n = _start - 1; (n >= firstEnabled && VisCount < MinVisibleCount); n--)
                {
                    if (hudCollectionList[n].Enabled)
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
            VisCount = 0;

            for (int n = _end; n >= firstEnabled; n--)
            {
                if (hudCollectionList[n].Enabled)
                {
                    TElement element = hudCollectionList[n].Element;

                    if (length > 0f || VisCount < MinVisibleCount)
                    {
                        range.X = n;
                        length -= element.Size[alignAxis];
                        VisCount++;
                    }
                    else
                        break;

                    length -= Spacing;
                }
            }

            if (EnabledCount > VisCount)
            {
                // Move ending index up until minimum visible requirment is met
                for (int n = _end + 1; (n < hudCollectionList.Count && VisCount < MinVisibleCount); n++)
                {
                    if (hudCollectionList[n].Enabled)
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
        /// Calculates the total size of all enabled elements in the scroll box, including spacing and
        /// any resizing that might be required.
        /// </summary>
        protected override Vector2 GetVisibleTotalSize()
        {
            Vector2 newSize = new Vector2();

            for (int n = 0; n < hudCollectionList.Count; n++)
            {
                TElementContainer container = hudCollectionList[n];

                if (container.Enabled)
                {
                    Vector2 elementSize = container.Element.Size;

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
        /// Returns the number of enabled elements before the one at the given index
        /// </summary>
        private int GetVisibleIndex(int index)
        {
            int count = 0;

            for (int n = 0; n < index; n++)
            {
                if (hudCollectionList[n].Enabled)
                    count++;
            }

            return count;
        }

        private float GetMinScrollOffset(int index, bool getEnd)
        {
            if (hudCollectionList.Count > 0)
            {
                firstEnabled = MathHelper.Clamp(firstEnabled, 0, hudCollectionList.Count - 1);
                float offset;

                if (getEnd)
                    offset = -(cachedSize[alignAxis] - cachedPadding[alignAxis] + Spacing);
                else
                    offset = -(hudCollectionList[firstEnabled].Element.Size[alignAxis] + Spacing);

                for (int i = 0; i <= index && i < hudCollectionList.Count; i++)
                {
                    if (hudCollectionList[i].Enabled)
                        offset += hudCollectionList[i].Element.Size[alignAxis] + Spacing;
                }

                return Math.Max(offset, 0f);
            }
            else
                return 0f;
        }

        public override void GetUpdateAccessors(List<HudUpdateAccessors> UpdateActions, byte treeDepth)
        {
            int preloadRange = Math.Max((End - Start) * 2, 10),
                preloadStart = MathHelper.Clamp(Start - preloadRange, 0, hudCollectionList.Count - 1),
                preloadCount = MathHelper.Clamp((End + preloadRange) - preloadStart, 0, hudCollectionList.Count - preloadStart);

            NodeUtils.SetNodesState<TElementContainer, TElement>
                (HudElementStates.CanPreload, true, hudCollectionList, 0, hudCollectionList.Count);
            NodeUtils.SetNodesState<TElementContainer, TElement>
                (HudElementStates.CanPreload | HudElementStates.IsSelectivelyMasked, false, hudCollectionList, preloadStart, preloadCount);

            base.GetUpdateAccessors(UpdateActions, treeDepth);
        }
    }

    /// <summary>
    /// Scrollable list of hud elements. Can be oriented vertically or horizontally. Min/Max size determines
    /// the maximum size of scrollbox elements as well as the scrollbox itself.
    /// </summary>
    public class ScrollBox<TElementContainer> : ScrollBox<TElementContainer, HudElementBase>
        where TElementContainer : IScrollBoxEntry<HudElementBase>, new()
    {
        public ScrollBox(bool alignVertical, HudParentBase parent = null) : base(alignVertical, parent)
        { }
    }

    /// <summary>
    /// Scrollable list of hud elements. Can be oriented vertically or horizontally. Min/Max size determines
    /// the maximum size of scrollbox elements as well as the scrollbox itself.
    /// </summary>
    public class ScrollBox : ScrollBox<ScrollBoxEntry>
    {
        public ScrollBox(bool alignVertical, HudParentBase parent = null) : base(alignVertical, parent)
        { }
    }
}
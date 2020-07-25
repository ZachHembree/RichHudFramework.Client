using System.Collections.Generic;
using VRageMath;
using System;
using System.Collections;

namespace RichHudFramework.UI
{
    /// <summary>
    /// Used to control sizing behavior of HudChain members and the containing chain element itself. The align axis
    /// is the axis chain elements are arranged on; the off axis is the other axis. When vertically aligned, Y is 
    /// the align axis and X is the off axis. Otherwise, it's reversed.
    /// </summary>
    public enum HudChainSizingModes : int
    {
        /// <summary>
        /// In this mode, the element will not attempt to resize chain members. The size of the element
		/// will be clamped between its minimum and maximum sizes, provided neither of those value are 
        /// less than the total size of the elements in the chain.
        /// </summary>
        Fixed = 0,

        /// <summary>
        /// In this mode, the element will automatically shrink/expand to fit its contents (plus padding).
        /// </summary>
        FitToMembers = 1,

        /// <summary>
        /// If this flag is set, then the size of chain members on the off axis will be clamped. 
        /// Superceeds FitToMembers.
        /// </summary>
        ClampMembersOffAxis = 2,

        /// <summary>
        /// If this flag is set, then the size of chain members on the align axis will be clamped. 
        /// Superceeds FitToMembers.
        /// </summary>
        ClampMembersAlignAxis = 4,

        /// <summary>
        /// In this mode, chain members will be clamped between the set min/max size on both axes. Superceeds FitToMembers.
        /// </summary>
        ClampMembersToBox = ClampMembersOffAxis | ClampMembersAlignAxis,

        /// <summary>
        /// If this flag is set, chain members will be automatically resized to fill the chain along the off axis. 
        /// Superceeds ClampMembersOffAxis and FitToMembers.
        /// </summary>
        FitMembersOffAxis = 8,

        /// <summary>
        /// If this flag is set, then the size of chain members on the align axis will be set to the maximum size. 
        /// Superceeds ClampMembersAlignAxis and FitToMembers.
        /// </summary>
        FitMembersAlignAxis = 16,

        /// <summary>
        /// In this mode, chain members will be set to the maximum size on both axes. Superceeds ClampMembersToBox.
        /// </summary>
        FitMembersToBox = FitMembersOffAxis | FitMembersAlignAxis,
    }

    /// <summary>
    /// HUD element used to organize other elements into straight lines, either horizontal or vertical. Min/Max size
    /// determines the minimum and maximum size of chain members.
    /// </summary>
    /*
     Rules:
        1) Chain members must fit inside the chain.
        2) Members must be positioned in the chain.
        3) Members are assumed to be compatible with the specified sizing mode. Otherwise the behavior is undefined
        and incorrect positioning and sizing will occur.
     */
    public class HudChain<T> : HudElementBase, IEnumerable<T> where T : HudElementBase
    {
        /// <summary>
        /// UI elements in the chain
        /// </summary>
        public IReadOnlyList<T> ChainElements => chainElements;

        /// <summary>
        /// Used to allow the addition of child elements using collection-initializer syntax in
        /// conjunction with normal initializers.
        /// </summary>
        public HudChain<T> ChainContainer => this;

        /// <summary>
        /// Width of the chain
        /// </summary>
        public override float Width
        {
            set
            {
                _maxSize.X = value / _scale;

                if (value > Padding.X)
                    value -= Padding.X;

                _absoluteWidth = value / _scale;
            }
        }

        /// <summary>
        /// Height of the chain
        /// </summary>
        public override float Height
        {
            set
            {
                _maxSize.Y = value / _scale;

                if (value > Padding.Y)
                    value -= Padding.Y;

                _absoluteHeight = value / _scale;
            }
        }

        /// <summary>
        /// Maximum chain member size. If no maximum is set, then the currently set size will be used as the maximum.
        /// </summary>
        public Vector2 MaximumSize { get { return _maxSize * _scale; } set { _maxSize = value / _scale; } }

        /// <summary>
        /// Minimum allowable member size.
        /// </summary>
        public Vector2 MinimumSize { get { return _minSize * _scale; } set { _minSize = value / _scale; } }

        /// <summary>
        /// Distance between chain elements along their axis of alignment.
        /// </summary>
        public float Spacing { get { return _spacing * _scale; } set { _spacing = value / _scale; } }

        /// <summary>
        /// Determines how/if the chain will attempt to resize member elements. Default sizing mode is 
        /// HudChainSizingModes.FitToMembers.
        /// </summary>
        public HudChainSizingModes SizingMode { get; set; }

        /// <summary>
        /// Determines whether or not chain elements will be aligned vertically.
        /// </summary>
        public bool AlignVertical => alignAxis == 1;

        /// <summary>
        /// UI elements in the chain
        /// </summary>
        protected readonly List<T> chainElements;

        protected float _spacing;
        protected int alignAxis, offAxis;
        protected Vector2 _maxSize, _minSize;

        public HudChain(bool alignVertical, IHudParent parent = null) : base(parent)
        {
            Spacing = 0f;
            chainElements = new List<T>();
            SizingMode = HudChainSizingModes.FitToMembers;

            if (alignVertical)
            {
                alignAxis = 1;
                offAxis = 0;
            }
            else
            {
                alignAxis = 0;
                offAxis = 1;
            }
        }

        public IEnumerator<T> GetEnumerator() =>
            chainElements.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();

        /// <summary>
        /// Adds an element of type <see cref="T"/> to the chain.
        /// </summary>
        public virtual void Add(T chainElement)
        {
            blockChildRegistration = true;

            chainElement.Register(this);

            if (chainElement.Parent != this)
                throw new Exception("HUD Element Registration Failed.");

            chainElements.Add(chainElement);

            blockChildRegistration = false;
        }

        /// <summary>
        /// Add the given range to the end of the chain.
        /// </summary>
        public virtual void AddRange(IList<T> newChainElements)
        {
            blockChildRegistration = true;

            for (int n = 0; n < newChainElements.Count; n++)
            {
                newChainElements[n].Register(this);

                if (newChainElements[n].Parent != this)
                    throw new Exception("HUD Element Registration Failed.");
            }

            chainElements.AddRange(newChainElements);
            blockChildRegistration = false;
        }

        /// <summary>
        /// Insert the given range into the chain.
        /// </summary>
        public virtual void InsertRange(int index, IList<T> newChainElements)
        {
            blockChildRegistration = true;

            for (int n = 0; n < newChainElements.Count; n++)
            {
                newChainElements[n].Register(this);

                if (newChainElements[n].Parent?.ID != this)
                    throw new Exception("HUD Element Registration Failed.");
            }

            chainElements.InsertRange(index, newChainElements);
            blockChildRegistration = false;
        }

        /// <summary>
        /// Removes the specified element from the chain.
        /// </summary>
        public void Remove(T chainElement)
        {
            if (chainElement.Parent == this)
            {
                int index = chainElements.FindIndex(x => x == chainElement);

                if (index != -1)
                {
                    chainElement.Unregister();
                    chainElements.RemoveAt(index);
                }
            }
        }

        /// <summary>
        /// Removes the chain member that meets the conditions required by the predicate.
        /// </summary>
        public void Remove(Func<T, bool> predicate)
        {
            int index = chainElements.FindIndex(x => predicate(x));
            RemoveAt(index);
        }

        /// <summary>
        /// Remove the chain element at the given index.
        /// </summary>
        public virtual void RemoveAt(int index)
        {
            if (chainElements[index].Parent == this)
            {
                blockChildRegistration = true;

                chainElements[index].Unregister();
                chainElements.RemoveAt(index);

                blockChildRegistration = false;
            }
        }

        /// <summary>
        /// Removes the specfied range from the chain. Normal child elements not affected.
        /// </summary>
        public virtual void RemoveRange(int index, int count)
        {
            blockChildRegistration = true;

            for (int n = index; n < index + count; n++)
                chainElements[n].Unregister();

            chainElements.RemoveRange(index, count);
            blockChildRegistration = false;
        }

        /// <summary>
        /// Remove all elements in the HudChain. Does not affect normal child elements.
        /// </summary>
        public virtual void ClearChain()
        {
            blockChildRegistration = true;

            for (int n = 0; n < chainElements.Count; n++)
                chainElements[n].Unregister();

            chainElements.Clear();
            blockChildRegistration = false;
        }

        /// <summary>
        /// Finds the chain member that meets the conditions required by the predicate.
        /// </summary>
        public T Find(Func<T, bool> predicate)
        {
            return chainElements.Find(x => predicate(x));
        }

        /// <summary>
        /// Finds the index of the chain member that meets the conditions required by the predicate.
        /// </summary>
        public int FindIndex(Func<T, bool> predicate)
        {
            return chainElements.FindIndex(x => predicate(x));
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
                            chainElements.RemoveAt(index);
                    }
                }
            }
        }

        public override void BeforeInput(HudLayers layer)
        {
            for (int n = children.Count - 1; n >= 0; n--)
            {
                if (children[n].Visible)
                    children[n].BeforeInput(layer);
            }

            // Add extra loop for chain elements
            for (int n = chainElements.Count - 1; n >= 0; n--)
            {
                if (chainElements[n].Visible)
                    chainElements[n].BeforeInput(layer);
            }

            if (_zOffset == layer)
            {
                UpdateMouseInput();
            }
        }

        public override void BeforeLayout(bool refresh)
        {
            UpdateCache();
            Layout();

            // Add extra loop for chain elements
            for (int n = 0; n < chainElements.Count; n++)
            {
                if (chainElements[n].Visible || refresh)
                    chainElements[n].BeforeLayout(refresh);
            }

            for (int n = 0; n < children.Count; n++)
            {
                if (children[n].Visible || refresh)
                    children[n].BeforeLayout(refresh);
            }
        }

        public override void BeforeDraw(HudLayers layer, ref MatrixD matrix)
        {
            if (_zOffset == layer)
                Draw(ref matrix);

            // Add extra loop for chain elements
            for (int n = 0; n < chainElements.Count; n++)
            {
                if (chainElements[n].Visible)
                    chainElements[n].BeforeDraw(layer, ref matrix);
            }

            for (int n = 0; n < children.Count; n++)
            {
                if (children[n].Visible)
                    children[n].BeforeDraw(layer, ref matrix);
            }
        }

        protected override void Layout()
        {
            Vector2 visibleTotalSize = GetVisibleTotalSize(), 
                elementMin = MinimumSize - cachedPadding, 
                elementMax = MaximumSize - cachedPadding;

            ClampElementSizeRange(visibleTotalSize, ref elementMin, ref elementMax);

            Vector2 newSize = GetNewSize(elementMax, visibleTotalSize);

            cachedSize = newSize;
            _absoluteWidth = cachedSize.X / _scale;
            _absoluteHeight = cachedSize.Y / _scale;
            cachedSize += cachedPadding;

            // Calculate member start offset
            Vector2 startOffset = Vector2.Zero;

            if (alignAxis == 1)
                startOffset.Y = newSize.Y / 2f;
            else
                startOffset.X = -newSize.X / 2f;

            UpdateMemberOffsets(startOffset, cachedPadding, elementMin, elementMax);
        }

        protected Vector2 GetNewSize(Vector2 elementMax, Vector2 totalSize)
        {
            Vector2 newSize;

            if (SizingMode == HudChainSizingModes.Fixed || SizingMode == HudChainSizingModes.FitToMembers)
            {
                newSize = elementMax;
            }
            else // if FitMembersToBox or ClampMembersToBox
            {
                newSize = Vector2.Zero;
                newSize[offAxis] = elementMax[offAxis];
                newSize[alignAxis] = totalSize[alignAxis];
            }

            return newSize;
        }

        /// <summary>
        /// Clamps minimum and maximum element sizes based on sizing configuration.
        /// </summary>
        protected void ClampElementSizeRange(Vector2 totalSize, ref Vector2 min, ref Vector2 max)
        {
            min = Vector2.Max(Vector2.Zero, min);
            max = Vector2.Max(Vector2.Zero, max);

            if (SizingMode == HudChainSizingModes.Fixed)
            {
                min = Vector2.Max(min, totalSize);
                max = Vector2.Max(min, max);
            }
            else if (SizingMode == HudChainSizingModes.FitToMembers)
            {
                min = totalSize;
                max = totalSize;
            }
            else // if FitMembersToBox or ClampMembersToBox
            {
                max = Vector2.Max(min, max);
                min = Vector2.Min(min, max);
            }
        }

        /// <summary>
        /// Updates chain member offsets to ensure that they're in a straight line.
        /// </summary>
        protected void UpdateMemberOffsets(Vector2 offset, Vector2 padding, Vector2 minSize, Vector2 maxSize)
        {
            Vector2 alignMask = new Vector2(offAxis, -alignAxis), offMask = new Vector2(alignAxis, -offAxis);
            ParentAlignments left = (ParentAlignments)((int)(ParentAlignments.Left) * (2 - alignAxis)),
                right = (ParentAlignments)((int)(ParentAlignments.Right) * (2 - alignAxis)),
                bitmask = left | right;

            for (int n = 0; n < chainElements.Count; n++)
            {
                if (chainElements[n].Visible)
                {
                    // Calculate element size
                    Vector2 elementSize = chainElements[n].Size;

                    if ((SizingMode & HudChainSizingModes.FitMembersOffAxis) > 0)
                        elementSize[offAxis] = maxSize[offAxis];
                    else if ((SizingMode & HudChainSizingModes.ClampMembersOffAxis) > 0)
                        elementSize[offAxis] = MathHelper.Clamp(elementSize[offAxis], minSize[offAxis], maxSize[offAxis]);

                    if ((SizingMode & HudChainSizingModes.FitMembersAlignAxis) > 0)
                        elementSize[alignAxis] = maxSize[alignAxis];
                    else if ((SizingMode & HudChainSizingModes.ClampMembersAlignAxis) > 0)
                        elementSize[alignAxis] = MathHelper.Clamp(elementSize[alignAxis], minSize[alignAxis], maxSize[alignAxis]);

                    // Enforce alignment restrictions
                    chainElements[n].ParentAlignment &= bitmask;
                    chainElements[n].ParentAlignment |= ParentAlignments.Inner;

                    // Calculate element offset
                    Vector2 newOffset = offset + (elementSize * alignMask * .5f);

                    if ((chainElements[n].ParentAlignment & left) == left)
                    {
                        newOffset += padding * offMask * .5f;
                    }
                    else if ((chainElements[n].ParentAlignment & right) == right)
                    {
                        newOffset -= padding * offMask * .5f;
                    }

                    // Apply changes
                    chainElements[n].Size = elementSize;
                    chainElements[n].Offset = newOffset;

                    // Move offset down for the next element
                    elementSize[alignAxis] += Spacing;
                    offset += elementSize * alignMask;
                }
            }
        }

        /// <summary>
        /// Calculates the total size of all visible elements in the chain, including spacing.
        /// </summary>
        protected Vector2 GetVisibleTotalSize()
        {
            Vector2 newSize = new Vector2();

            for (int n = 0; n < chainElements.Count; n++)
            {
                if (chainElements[n].Visible)
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
    }
}

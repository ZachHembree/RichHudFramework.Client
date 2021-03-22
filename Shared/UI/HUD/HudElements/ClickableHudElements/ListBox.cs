using System;
using System.Text;
using VRage;
using VRageMath;
using System.Collections.Generic;
using RichHudFramework.UI.Rendering;
using GlyphFormatMembers = VRage.MyTuple<byte, float, VRageMath.Vector2I, VRageMath.Color>;
using ApiMemberAccessor = System.Func<object, int, object>;
using System.Collections;

namespace RichHudFramework.UI
{
    using CollectionData = MyTuple<Func<int, ApiMemberAccessor>, Func<int>>;
    using RichStringMembers = MyTuple<StringBuilder, GlyphFormatMembers>;

    public enum ListBoxAccessors : int
    {
        /// <summary>
        /// CollectionData
        /// </summary>
        ListMembers = 1,

        /// <summary>
        /// in: MyTuple<IList<RichStringMembers>, T>, out: ApiMemberAccessor
        /// </summary>
        Add = 2,

        /// <summary>
        /// out: ListBoxEntry
        /// </summary>
        Selection = 3,

        /// <summary>
        /// out: int
        /// </summary>
        SelectionIndex = 4,

        /// <summary>
        /// in: T (AssocObject)
        /// </summary>
        SetSelectionAtData = 5,

        /// <summary>
        /// in: MyTuple<int, IList<RichStringMembers>, T>
        /// </summary>
        Insert = 6,

        /// <summary>
        /// in: ListBoxEntry, out: bool
        /// </summary>
        Remove = 7,

        /// <summary>
        /// in: int
        /// </summary>
        RemoveAt = 8,

        /// <summary>
        /// void
        /// </summary>
        ClearEntries = 9
    }

    /// <summary>
    /// Generic scrollable list of text elements. Allows use of custom entry element types.
    /// Each list entry is associated with a value of type T.
    /// </summary>
    public class ListBox<TElementContainer, TElement, TValue>
        : SelectionBox<ScrollBox<TElementContainer, TElement>, TElementContainer, TElement, TValue>, IClickableElement
        where TElementContainer : class, IListBoxEntry<TElement, TValue>, new()
        where TElement : HudElementBase, ILabelElement
    {
        /// <summary>
        /// Background color
        /// </summary>
        public Color Color { get { return hudChain.Color; } set { hudChain.Color = value; } }

        /// <summary>
        /// Color of the slider bar
        /// </summary>
        public Color BarColor { get { return hudChain.BarColor; } set { hudChain.BarColor = value; } }

        /// <summary>
        /// Bar color when moused over
        /// </summary>
        public Color BarHighlight { get { return hudChain.BarHighlight; } set { hudChain.BarHighlight = value; } }

        /// <summary>
        /// Color of the slider box when not moused over
        /// </summary>
        public Color SliderColor { get { return hudChain.SliderColor; } set { hudChain.SliderColor = value; } }

        /// <summary>
        /// Color of the slider button when moused over
        /// </summary>
        public Color SliderHighlight { get { return hudChain.SliderHighlight; } set { hudChain.SliderHighlight = value; } }

        /// <summary>
        /// Minimum number of elements visible in the list at any given time.
        /// </summary>
        public int MinVisibleCount { get { return hudChain.MinVisibleCount; } set { hudChain.MinVisibleCount = value; } }

        protected override Vector2I ListRange => new Vector2I(hudChain.Start, hudChain.End);

        protected override Vector2 ListSize
        {
            get
            {
                Vector2 listSize = hudChain.Size;
                listSize.X -= hudChain.ScrollBar.Width;

                return listSize;
            }
        }

        protected override Vector2 ListPos
        {
            get
            {
                Vector2 listPos = hudChain.Position;
                listPos.X -= hudChain.ScrollBar.Width;

                return listPos;
            }
        }

        public ListBox(HudParentBase parent) : base(parent)
        {
            hudChain.MinVisibleCount = 6;
            hudChain.Padding = new Vector2(0f, 8f);
        }

        public ListBox() : this(null)
        { }

        /// <summary>
        /// Update indices for selections, highlight and focus
        /// </summary>
        protected override void UpdateSelection()
        {
            if (listInput.KeyboardScroll)
            {
                if (listInput.HighlightIndex < hudChain.Start)
                    hudChain.Start = listInput.HighlightIndex;
                else if (listInput.HighlightIndex > hudChain.End)
                    hudChain.End = listInput.HighlightIndex;
            }

            UpdateSelectionPositions();
            UpdateSelectionFormatting();
        }

        protected override void Layout()
        {
            hudChain.Height = LineHeight * hudChain.MinVisibleCount;
        }

        protected override void Draw()
        {
            Size = hudChain.Size + Padding;
        }
    }

    /// <summary>
    /// Scrollable list of text elements. Each list entry is associated with a value of type T.
    /// </summary>
    public class ListBox<TValue> : ListBox<ListBoxLabel<TValue>, Label, TValue>
    {
        public ListBox(HudParentBase parent) : base(parent)
        { }

        public ListBox() : base(null)
        { }
    }

}
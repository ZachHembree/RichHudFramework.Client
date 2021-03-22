using System;
using System.Collections;
using System.Collections.Generic;
using VRageMath;
using VRage;

namespace RichHudFramework.UI
{
    using Rendering;

    /// <summary>
    /// Generic indented collapsable list. Allows use of custom entry element types. 
    /// Designed to fit in with SE UI elements.
    /// </summary>
    public class TreeBox<TElementContainer, TElement, TValue> 
        : HudElementBase, IClickableElement, IEntryBox<TValue, TElementContainer, TElement>
        where TElementContainer : class, IListBoxEntry<TElement, TValue>, new()
        where TElement : HudElementBase, IClickableElement, ILabelElement
    {
        /// <summary>
        /// Invoked when an entry is selected.
        /// </summary>
        public event EventHandler SelectionChanged
        {
            add { selectionBox.SelectionChanged += value; }
            remove { selectionBox.SelectionChanged -= value; }
        }

        /// <summary>
        /// List of entries in the treebox.
        /// </summary>
        public IReadOnlyList<TElementContainer> ListEntries => selectionBox.ListEntries;

        /// <summary>
        /// Used to allow the addition of list entries using collection-initializer syntax in
        /// conjunction with normal initializers.
        /// </summary>
        public TreeBox<TElementContainer, TElement, TValue> ListContainer => this;

        /// <summary>
        /// If true, then the dropdown list will be open
        /// </summary>
        public bool ListOpen { get; protected set; }

        /// <summary>
        /// Height of the treebox in pixels.
        /// </summary>
        public override float Height
        {
            get
            {
                if (!ListOpen)
                    return display.Height + Padding.Y;
                else
                    return display.Height + selectionBox.Height + Padding.Y;
            }
            set
            {
                if (Padding.Y < value)
                    value -= Padding.Y;

                if (!ListOpen)
                {
                    display.Height = value;
                    selectionBox.LineHeight = value;
                }
            }
        }

        /// <summary>
        /// Name of the element as rendered on the display
        /// </summary>
        public RichText Name { get { return display.Name; } set { display.Name = value; } }

        /// <summary>
        /// Default format for member text;
        /// </summary>
        public GlyphFormat Format { get { return display.Format; } set { display.Format = value; selectionBox.Format = value; } }

        /// <summary>
        /// Text formatting used for entries that have input focus
        /// </summary>
        public GlyphFormat FocusFormat { get { return selectionBox.FocusFormat; } set { selectionBox.FocusFormat = value; } }

        /// <summary>
        /// Determines the color of the header's background/
        /// </summary>
        public Color HeaderColor { get { return display.Color; } set { display.Color = value; } }

        /// <summary>
        /// Default background color of the highlight box
        /// </summary>
        public Color HighlightColor { get { return selectionBox.HighlightColor; } set { selectionBox.HighlightColor = value; } }

        /// <summary>
        /// Background color used for selection/highlighting when the list has input focus
        /// </summary>
        public Color FocusColor { get { return selectionBox.FocusColor; } set { selectionBox.FocusColor = value; } }

        /// <summary>
        /// Background color used for selection/highlighting when the list has input focus
        /// </summary>
        public Color TabColor { get { return selectionBox.TabColor; } set { selectionBox.TabColor = value; } }

        /// <summary>
        /// Padding applied to the highlight box.
        /// </summary>
        public Vector2 HighlightPadding { get { return selectionBox.HighlightPadding; } set { selectionBox.HighlightPadding = value; } }

        /// <summary>
        /// Current selection. Null if empty.
        /// </summary>
        public TElementContainer Selection => selectionBox.Selection;

        /// <summary>
        /// Size of the entry collection.
        /// </summary>
        public int Count => selectionBox.Count;

        /// <summary>
        /// Determines how far to the right list members should be offset from the position of the header.
        /// </summary>
        public float IndentSize 
        { 
            get { return selectionBox.Padding.X; } 
            set 
            {
                selectionBox.Padding = new Vector2(value, selectionBox.Padding.Y);
                selectionBox.Offset = selectionBox.Padding / 2f;
            } 
         }

        /// <summary>
        /// Handles mouse input for the header.
        /// </summary>
        public IMouseInput MouseInput => display.MouseInput;

        public HudElementBase Display => display;

        public readonly SelectionBox<HudChain<TElementContainer, TElement>, TElementContainer, TElement, TValue> selectionBox;

        protected readonly TreeBoxDisplay display;

        public TreeBox(HudParentBase parent) : base(parent)
        {
            display = new TreeBoxDisplay(this)
            {
                ParentAlignment = ParentAlignments.Top | ParentAlignments.InnerV | ParentAlignments.UsePadding,
                DimAlignment = DimAlignments.Width | DimAlignments.IgnorePadding
            };

            selectionBox = new SelectionBox<HudChain<TElementContainer, TElement>, TElementContainer, TElement, TValue>()
            {
                Visible = false,
                ParentAlignment = ParentAlignments.Bottom,
                HighlightPadding = Vector2.Zero
            };
            selectionBox.Register(display, false, true);

            selectionBox.border.Visible = false;
            selectionBox.hudChain.SizingMode = 
                HudChainSizingModes.FitMembersBoth | 
                HudChainSizingModes.ClampChainOffAxis | 
                HudChainSizingModes.FitChainAlignAxis;

            Size = new Vector2(200f, 34f);
            IndentSize = 20f;

            Format = GlyphFormat.Blueish;
            selectionBox.FocusFormat = Format.WithColor(TerminalFormatting.Charcoal);

            display.Name = "NewTreeBox";
            display.MouseInput.LeftClicked += ToggleList;
        }

        public TreeBox() : this(null)
        { }

        /// <summary>
        /// Sets the selection to the member associated with the given object.
        /// </summary>
        public void SetSelection(TValue assocMember) =>
            selectionBox.SetSelection(assocMember);

        /// <summary>
        /// Sets the selection to the specified entry.
        /// </summary>
        public void SetSelection(TElementContainer member) =>
            selectionBox.SetSelection(member);

        /// <summary>
        /// Clears the current selection.
        /// </summary>
        public void ClearSelection() =>
            selectionBox.ClearSelection();

        /// <summary>
        /// Adds a new member to the tree box with the given name and associated
        /// object.
        /// </summary>
        public TElementContainer Add(RichText name, TValue assocMember, bool enabled = true) =>
            selectionBox.Add(name, assocMember, enabled);

        /// <summary>
        /// Adds the given range of entries to the tree box.
        /// </summary>
        public void AddRange(IReadOnlyList<MyTuple<RichText, TValue, bool>> entries) =>
            selectionBox.AddRange(entries);

        /// <summary>
        /// Inserts an entry at the given index.
        /// </summary>
        public void Insert(int index, RichText name, TValue assocMember, bool enabled = true) =>
            selectionBox.Insert(index, name, assocMember, enabled);

        /// <summary>
        /// Removes the member at the given index from the tree box.
        /// </summary>
        public void RemoveAt(int index) =>
            selectionBox.RemoveAt(index);

        /// <summary>
        /// Removes the specified range of indices from the tree box.
        /// </summary>
        public void RemoveRange(int index, int count) =>
            selectionBox.RemoveRange(index, count);

        /// <summary>
        /// Clears the current selection
        /// </summary>
        public void ClearEntries() =>
            selectionBox.ClearEntries();

        private void ToggleList(object sender, EventArgs args)
        {
            if (!ListOpen)
                OpenList();
            else
                CloseList();
        }

        public void OpenList()
        {
            selectionBox.Visible = true;
            display.Open = true;
            ListOpen = true;
        }

        public void CloseList()
        {
            selectionBox.Visible = false;
            display.Open = false;
            ListOpen = false;
        }

        protected override void Layout()
        {
            selectionBox.Visible = ListOpen;

            if (ListOpen)
            {
                selectionBox.Width = Width - IndentSize - Padding.X;
                selectionBox.Offset = new Vector2(IndentSize, 0f);
            }
        }

        public IEnumerator<TElementContainer> GetEnumerator() =>
            selectionBox.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            selectionBox.GetEnumerator();

        /// <summary>
        /// Modified dropdown header with a rotating arrow on the left side indicating
        /// whether the list is open.
        /// </summary>
        protected class TreeBoxDisplay : HudElementBase
        {
            public RichText Name { get { return name.Text; } set { name.Text = value; } }

            public GlyphFormat Format { get { return name.Format; } set { name.Format = value; } }

            public Color Color { get { return background.Color; } set { background.Color = value; } }

            public IMouseInput MouseInput => mouseInput;

            public bool Open
            {
                get { return open; }
                set
                {
                    open = value;

                    if (open)
                        arrow.Material = downArrow;
                    else
                        arrow.Material = rightArrow;
                }
            }

            private bool open;

            public readonly Label name;
            private readonly TexturedBox arrow, divider, background;
            private readonly HudChain layout;
            private readonly MouseInputElement mouseInput;

            private static readonly Material 
                downArrow = new Material("RichHudDownArrow", new Vector2(64f, 64f)), 
                rightArrow = new Material("RichHudRightArrow", new Vector2(64f, 64f));

            public TreeBoxDisplay(HudParentBase parent = null) : base(parent)
            {
                background = new TexturedBox(this)
                {
                    Color = TerminalFormatting.EbonyClay,
                    DimAlignment = DimAlignments.Both,
                };

                name = new Label()
                {
                    AutoResize = false,
                    Padding = new Vector2(10f, 0f),
                    Format = GlyphFormat.Blueish.WithSize(1.1f),
                };

                divider = new TexturedBox()
                {
                    Padding = new Vector2(2f, 6f),
                    Size = new Vector2(2f, 39f),
                    Color = new Color(104, 113, 120),
                };

                arrow = new TexturedBox()
                {
                    Width = 20f,
                    Padding = new Vector2(8f, 0f),
                    MatAlignment = MaterialAlignment.FitHorizontal,
                    Color = new Color(227, 230, 233),
                    Material = rightArrow,
                };

                layout = new HudChain(false, this)
                {
                    SizingMode = HudChainSizingModes.FitMembersOffAxis | HudChainSizingModes.FitChainBoth,
                    DimAlignment = DimAlignments.Height | DimAlignments.IgnorePadding,
                    CollectionContainer = { arrow, divider, name }
                };

                mouseInput = new MouseInputElement(this)
                {
                    DimAlignment = DimAlignments.Both
                };
            }

            protected override void Layout()
            {
                name.Width = (Width - Padding.X) - divider.Width - arrow.Width;
            }
        }
    }

    /// <summary>
    /// Indented, collapsable list. Designed to fit in with SE UI elements.
    /// </summary>
    public class TreeBox<TValue> : TreeBox<ListBoxEntry<TValue>, LabelButton, TValue>
    {
        public TreeBox(HudParentBase parent) : base(parent)
        { }

        public TreeBox() : base(null)
        { }
    }
}
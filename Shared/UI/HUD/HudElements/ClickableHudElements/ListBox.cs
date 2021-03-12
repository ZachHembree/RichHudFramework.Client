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
        : HudElementBase, IEntryBox<TValue, TElementContainer, TElement>
        where TElementContainer : class, IListBoxEntry<TElement, TValue>, new()
        where TElement : HudElementBase, IClickableElement, ILabelElement
    {
        /// <summary>
        /// Invoked when an entry is selected.
        /// </summary>
        public event EventHandler SelectionChanged;

        /// <summary>
        /// Used to allow the addition of list entries using collection-initializer syntax in
        /// conjunction with normal initializers.
        /// </summary>
        public ListBox<TElementContainer, TElement, TValue> ListContainer => this;

        /// <summary>
        /// Read-only collection of list entries.
        /// </summary>
        public IReadOnlyList<TElementContainer> ListEntries => scrollBox.Collection;

        /// <summary>
        /// Read-only collection of list entries.
        /// </summary>
        public IReadOnlyHudCollection<TElementContainer, TElement> HudCollection => scrollBox;

        /// <summary>
        /// Background color
        /// </summary>
        public Color Color { get { return scrollBox.Color; } set { scrollBox.Color = value; } }

        /// <summary>
        /// Color of the slider bar
        /// </summary>
        public Color BarColor { get { return scrollBox.BarColor; } set { scrollBox.BarColor = value; } }

        /// <summary>
        /// Bar color when moused over
        /// </summary>
        public Color BarHighlight { get { return scrollBox.BarHighlight; } set { scrollBox.BarHighlight = value; } }

        /// <summary>
        /// Color of the slider box when not moused over
        /// </summary>
        public Color SliderColor { get { return scrollBox.SliderColor; } set { scrollBox.SliderColor = value; } }

        /// <summary>
        /// Color of the slider button when moused over
        /// </summary>
        public Color SliderHighlight { get { return scrollBox.SliderHighlight; } set { scrollBox.SliderHighlight = value; } }

        /// <summary>
        /// Background color of the highlight box
        /// </summary>
        public Color HighlightColor
        {
            get { return selectionBox.Color; }
            set
            {
                selectionBox.Color = value;
                highlight.Color = value;
            }
        }

        /// <summary>
        /// Color of the highlight box's tab
        /// </summary>
        public Color TabColor
        {
            get { return selectionBox.TabColor; }
            set
            {
                selectionBox.TabColor = value;
                highlight.TabColor = value;
            }
        }

        /// <summary>
        /// Padding applied to list members.
        /// </summary>
        public Vector2 MemberPadding
        {
            get { return _memberPadding; }
            set
            {
                _memberPadding = value;

                for (int n = 0; n < scrollBox.Collection.Count; n++)
                    scrollBox.Collection[n].Element.Padding = value;
            }
        }

        /// <summary>
        /// Padding applied to the highlight box.
        /// </summary>
        public Vector2 HighlightPadding { get; set; }

        /// <summary>
        /// Height of entries in the list.
        /// </summary>
        public float LineHeight 
        { 
            get { return scrollBox.MemberMaxSize.Y; } 
            set { scrollBox.MemberMaxSize = new Vector2(scrollBox.MemberMaxSize.X, value); } 
        }

        /// <summary>
        /// Default format for member text;
        /// </summary>
        public GlyphFormat Format { get; set; }

        /// <summary>
        /// Minimum number of elements visible in the list at any given time.
        /// </summary>
        public int MinVisibleCount { get { return scrollBox.MinVisibleCount; } set { scrollBox.MinVisibleCount = value; } }

        /// <summary>
        /// Current selection. Null if empty.
        /// </summary>
        public TElementContainer Selection => SelectionIndex != -1 ? scrollBox.Collection[SelectionIndex] : default(TElementContainer);

        /// <summary>
        /// Index of the current selection. -1 if empty.
        /// </summary>
        public int SelectionIndex { get; protected set; }

        public readonly ScrollBox<TElementContainer, TElement> scrollBox;
        protected readonly HighlightBox selectionBox, highlight;
        protected readonly BorderBox border;
        protected Vector2 _memberPadding;
        protected readonly ObjectPool<TElementContainer> entryPool;

        public ListBox(HudParentBase parent) : base(parent)
        {
            entryPool = new ObjectPool<TElementContainer>(GetNewEntry, ResetEntry);

            scrollBox = new ScrollBox<TElementContainer, TElement>(true, this)
            {
                SizingMode = HudChainSizingModes.FitMembersBoth | HudChainSizingModes.ClampChainOffAxis,
                DimAlignment = DimAlignments.Both | DimAlignments.IgnorePadding,
            };

            selectionBox = new HighlightBox(this);
            highlight = new HighlightBox(this);

            border = new BorderBox(scrollBox)
            {
                DimAlignment = DimAlignments.Both,
                Color = new Color(58, 68, 77),
                Thickness = 1f,
            };

            Format = GlyphFormat.White;
            Size = new Vector2(335f, 203f);

            HighlightPadding = new Vector2(12f, 6f);
            MemberPadding = new Vector2(20f, 6f);
            LineHeight = 30f;

            SelectionIndex = -1;
        }

        public ListBox() : this(null)
        { }

        public IEnumerator<TElementContainer> GetEnumerator() =>
            scrollBox.Collection.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();

        /// <summary>
        /// Adds a new member to the list box with the given name and associated
        /// object.
        /// </summary>
        public TElementContainer Add(RichText name, TValue assocMember, bool enabled = true)
        {
            TElementContainer entry = entryPool.Get();

            entry.Element.Text = name;
            entry.AssocMember = assocMember;
            entry.Enabled = enabled;
            scrollBox.Add(entry);

            return entry;
        }

        /// <summary>
        /// Adds the given range of entries to the list box.
        /// </summary>
        public void AddRange(IReadOnlyList<MyTuple<RichText, TValue, bool>> entries)
        {
            for (int n = 0; n < entries.Count; n++)
            {
                TElementContainer entry = entryPool.Get();

                entry.Element.Text = entries[n].Item1;
                entry.AssocMember = entries[n].Item2;
                entry.Enabled = entries[n].Item3;
                scrollBox.Add(entry);
            }
        }

        /// <summary>
        /// Inserts an entry at the given index.
        /// </summary>
        public void Insert(int index, RichText name, TValue assocMember, bool enabled = true)
        {
            TElementContainer entry = entryPool.Get();

            entry.Element.Text = name;
            entry.AssocMember = assocMember;
            entry.Enabled = enabled;
            scrollBox.Insert(index, entry);
        }

        /// <summary>
        /// Removes the member at the given index from the list box.
        /// </summary>
        public void RemoveAt(int index)
        {
            TElementContainer entry = scrollBox.Collection[index];
            scrollBox.RemoveAt(index, true);
            entryPool.Return(entry);
        }

        /// <summary>
        /// Removes the member at the given index from the list box.
        /// </summary>
        public bool Remove(TElementContainer entry)
        {
            if (scrollBox.Remove(entry, true))
            {
                entryPool.Return(entry);
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Removes the specified range of indices from the list box.
        /// </summary>
        public void RemoveRange(int index, int count)
        {
            for (int n = index; n < index + count; n++)
                entryPool.Return(scrollBox.Collection[n]);

            scrollBox.RemoveRange(index, count, true);
        }

        /// <summary>
        /// Removes all entries from the list box.
        /// </summary>
        public void ClearEntries()
        {
            for (int n = 0; n < scrollBox.Collection.Count; n++)
                entryPool.Return(scrollBox.Collection[n]);

            scrollBox.Clear(true);
        }

        /// <summary>
        /// Sets the selection to the member associated with the given object.
        /// </summary>
        public void SetSelectionAt(int index)
        {
            SelectionIndex = MathHelper.Clamp(index, 0, scrollBox.Count - 1);
            Selection.Enabled = true;
            SelectionChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Sets the selection to the member associated with the given object.
        /// </summary>
        public void SetSelection(TValue assocMember)
        {
            int index = scrollBox.FindIndex(x => assocMember.Equals(x.AssocMember));

            if (index != -1)
            {
                SelectionIndex = MathHelper.Clamp(index, 0, scrollBox.Count - 1);
                Selection.Enabled = true;
                SelectionChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Sets the selection to the specified entry.
        /// </summary>
        public void SetSelection(TElementContainer member)
        {
            int index = scrollBox.FindIndex(x => member.Equals(x));

            if (index != -1)
            {
                SelectionIndex = MathHelper.Clamp(index, 0, scrollBox.Count - 1);
                Selection.Enabled = true;
                SelectionChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private TElementContainer GetNewEntry()
        {
            var entry = new TElementContainer();
            entry.Element.Format = Format;
            entry.Element.Padding = _memberPadding;
            entry.Enabled = true;

            return entry;
        }

        private void ResetEntry(TElementContainer entry)
        {
            if (Selection == entry)
                SelectionIndex = -1;

            entry.Element.TextBoard.Clear();
            entry.Element.MouseInput.ClearSubscribers();
            entry.AssocMember = default(TValue);
            entry.Enabled = true;
        }

        protected override void HandleInput(Vector2 cursorPos)
        {
            // Make sure the selection box highlights the current selection
            if (Selection != null && Selection.Element.Visible)
            {
                selectionBox.Offset = Selection.Element.Position - selectionBox.Origin;
                selectionBox.Size = Selection.Element.Size;
                selectionBox.Visible = Selection.Element.Visible;
            }
            else
                selectionBox.Visible = false;

            highlight.Visible = false;

            for (int n = 0; n < scrollBox.Collection.Count; n++)
            {
                TElementContainer entry = scrollBox.Collection[n];

                if (entry.Element.IsMousedOver)
                {
                    highlight.Visible = true;
                    highlight.Size = entry.Element.Size;
                    highlight.Offset = entry.Element.Position - highlight.Origin;

                    if (SharedBinds.LeftButton.IsNewPressed)
                    {
                        SelectionIndex = n;
                        SelectionChanged?.Invoke(this, EventArgs.Empty);
                    }
                }
            }
        }

        protected override void Draw()
        {
            Size = scrollBox.Size + Padding;
        }

        public object GetOrSetMember(object data, int memberEnum)
        {
            var member = (ListBoxAccessors)memberEnum;

            switch (member)
            {
                case ListBoxAccessors.ListMembers:
                    return new CollectionData
                    (
                        x => scrollBox.Collection[x].GetOrSetMember,
                        () => scrollBox.Collection.Count
                     );
                case ListBoxAccessors.Add:
                    {
                        if (data is MyTuple<List<RichStringMembers>, TValue>)
                        {
                            var entryData = (MyTuple<List<RichStringMembers>, TValue>)data;
                            return (ApiMemberAccessor)Add(new RichText(entryData.Item1), entryData.Item2).GetOrSetMember;
                        }
                        else
                        {
                            var entryData = (MyTuple<IList<RichStringMembers>, TValue>)data;
                            var stringList = entryData.Item1 as List<RichStringMembers>;
                            return (ApiMemberAccessor)Add(new RichText(stringList), entryData.Item2).GetOrSetMember;
                        }
                    }
                case ListBoxAccessors.Selection:
                    {
                        if (data == null)
                            return Selection;
                        else
                            SetSelection(data as TElementContainer);

                        break;
                    }
                case ListBoxAccessors.SelectionIndex:
                    {
                        if (data == null)
                            return SelectionIndex;
                        else
                            SetSelectionAt((int)data); break;
                    }
                case ListBoxAccessors.SetSelectionAtData:
                    SetSelection((TValue)data); break;
                case ListBoxAccessors.Insert:
                    {
                        var entryData = (MyTuple<int, List<RichStringMembers>, TValue>)data;
                        Insert(entryData.Item1, new RichText(entryData.Item2), entryData.Item3);
                        break;
                    }
                case ListBoxAccessors.Remove:
                    return Remove(data as TElementContainer);
                case ListBoxAccessors.RemoveAt:
                    RemoveAt((int)data); break;
                case ListBoxAccessors.ClearEntries:
                    ClearEntries(); break;
            }

            return null;
        }

        /// <summary>
        /// A textured box with a white tab positioned on the left hand side.
        /// </summary>
        protected class HighlightBox : TexturedBox
        {
            public Color TabColor { get { return tabBoard.Color; } set { tabBoard.Color = value; } }

            private readonly MatBoard tabBoard;

            public HighlightBox(HudParentBase parent = null) : base(parent)
            {
                tabBoard = new MatBoard() { Color = new Color(223, 230, 236) };
                Color = new Color(34, 44, 53);
            }

            protected override void Layout()
            {
                hudBoard.Size = cachedSize - cachedPadding;
                tabBoard.Size = new Vector2(4f * Scale, cachedSize.Y - cachedPadding.Y);
            }

            protected override void Draw()
            {
                var ptw = HudSpace.PlaneToWorld;

                if (hudBoard.Color.A > 0)
                    hudBoard.Draw(cachedPosition, ref ptw);

                // Left align the tab
                Vector2 tabPos = cachedPosition;
                tabPos.X += (-hudBoard.Size.X + tabBoard.Size.X) / 2f;

                if (tabBoard.Color.A > 0)
                    tabBoard.Draw(tabPos, ref ptw);
            }
        }
    }

    /// <summary>
    /// Scrollable list of text elements. Each list entry is associated with a value of type T.
    /// </summary>
    public class ListBox<TValue> : ListBox<ListBoxEntry<TValue>, LabelButton, TValue>
    {
        public ListBox(HudParentBase parent) : base(parent)
        { }

        public ListBox() : base(null)
        { }
    }

}
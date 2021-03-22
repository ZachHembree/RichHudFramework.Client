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

    /// <summary>
    /// Generic base type for lists of selectable entries of fixed size.
    /// </summary>
    public class SelectionBox<THudChain, TElementContainer, TElement, TValue>
        : HudElementBase, IEntryBox<TValue, TElementContainer, TElement>, IClickableElement
        where THudChain : HudChain<TElementContainer, TElement>, new()
        where TElementContainer : class, IListBoxEntry<TElement, TValue>, new()
        where TElement : HudElementBase, ILabelElement
    {
        /// <summary>
        /// Invoked when an entry is selected.
        /// </summary>
        public event EventHandler SelectionChanged
        {
            add { listInput.SelectionChanged += value; }
            remove { listInput.SelectionChanged -= value; }
        }

        /// <summary>
        /// Used to allow the addition of list entries using collection-initializer syntax in
        /// conjunction with normal initializers.
        /// </summary>
        public SelectionBox<THudChain, TElementContainer, TElement, TValue> ListContainer => this;

        /// <summary>
        /// Read-only collection of list entries.
        /// </summary>
        public IReadOnlyList<TElementContainer> ListEntries => hudChain.Collection;

        /// <summary>
        /// Read-only collection of list entries.
        /// </summary>
        public IReadOnlyHudCollection<TElementContainer, TElement> HudCollection => hudChain;

        /// <summary>
        /// Default background color of the highlight box
        /// </summary>
        public Color HighlightColor { get; set; }

        /// <summary>
        /// Background color used for selection/highlighting when the list has input focus
        /// </summary>
        public Color FocusColor { get; set; }

        /// <summary>
        /// Color of the highlight box's tab
        /// </summary>
        public Color TabColor
        {
            get { return selectionBox.TabColor; }
            set
            {
                selectionBox.TabColor = value;
                highlightBox.TabColor = value;
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

                for (int n = 0; n < hudChain.Collection.Count; n++)
                    hudChain.Collection[n].Element.Padding = value;
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
            get { return hudChain.MemberMaxSize.Y; }
            set { hudChain.MemberMaxSize = new Vector2(hudChain.MemberMaxSize.X, value); }
        }

        /// <summary>
        /// Default format for member text;
        /// </summary>
        public GlyphFormat Format { get; set; }

        /// <summary>
        /// Text formatting used for entries that have input focus
        /// </summary>
        public GlyphFormat FocusFormat { get; set; }

        /// <summary>
        /// Current selection. Null if empty.
        /// </summary>
        public TElementContainer Selection => listInput.Selection;

        /// <summary>
        /// Index of the current selection. -1 if empty.
        /// </summary>
        public int SelectionIndex => listInput.SelectionIndex;

        /// <summary>
        /// Size of the entry collection.
        /// </summary>
        public int Count => hudChain.Count;

        public IMouseInput MouseInput => listInput;

        protected virtual Vector2I ListRange => new Vector2I(0, hudChain.Count - 1);

        protected virtual Vector2 ListSize => hudChain.Size;

        protected virtual Vector2 ListPos => hudChain.Position;

        public readonly THudChain hudChain;
        public readonly BorderBox border;
        protected readonly HighlightBox selectionBox, highlightBox;
        protected readonly ObjectPool<TElementContainer> entryPool;
        protected readonly ListInputElement<TElementContainer, TElement, TValue> listInput;
        protected Vector2 _memberPadding;

        public SelectionBox(HudParentBase parent) : base(parent)
        {
            entryPool = new ObjectPool<TElementContainer>(GetNewEntry, ResetEntry);

            hudChain = new THudChain() 
            {
                AlignVertical = true,
                SizingMode = HudChainSizingModes.FitMembersBoth | HudChainSizingModes.ClampChainOffAxis,
                DimAlignment = DimAlignments.Both | DimAlignments.IgnorePadding,
            };
            hudChain.Register(this);

            listInput = new ListInputElement<TElementContainer, TElement, TValue>(hudChain);
            selectionBox = new HighlightBox();
            highlightBox = new HighlightBox() { CanDrawTab = false };

            selectionBox.Register(this, false, true);
            highlightBox.Register(this, false, true);

            border = new BorderBox(hudChain)
            {
                DimAlignment = DimAlignments.Both,
                Color = new Color(58, 68, 77),
                Thickness = 1f,
            };

            HighlightColor = TerminalFormatting.Atomic;
            FocusColor = TerminalFormatting.Mint;

            Format = TerminalFormatting.ControlFormat;
            FocusFormat = TerminalFormatting.InvControlFormat;
            Size = new Vector2(335f, 203f);

            HighlightPadding = new Vector2(8f, 0f);
            MemberPadding = new Vector2(20f, 6f);
            LineHeight = 28f;
        }

        public SelectionBox() : this(null)
        { }

        public IEnumerator<TElementContainer> GetEnumerator() =>
            hudChain.Collection.GetEnumerator();

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
            hudChain.Add(entry);

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
                hudChain.Add(entry);
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
            hudChain.Insert(index, entry);
        }

        /// <summary>
        /// Removes the member at the given index from the list box.
        /// </summary>
        public void RemoveAt(int index)
        {
            TElementContainer entry = hudChain.Collection[index];
            hudChain.RemoveAt(index, true);
            entryPool.Return(entry);
        }

        /// <summary>
        /// Removes the member at the given index from the list box.
        /// </summary>
        public bool Remove(TElementContainer entry)
        {
            if (hudChain.Remove(entry, true))
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
                entryPool.Return(hudChain.Collection[n]);

            hudChain.RemoveRange(index, count, true);
        }

        /// <summary>
        /// Removes all entries from the list box.
        /// </summary>
        public void ClearEntries()
        {
            for (int n = 0; n < hudChain.Collection.Count; n++)
                entryPool.Return(hudChain.Collection[n]);

            hudChain.Clear(true);
        }

        /// <summary>
        /// Sets the selection to the member associated with the given object.
        /// </summary>
        public void SetSelectionAt(int index) =>
            listInput.SetSelectionAt(index);

        /// <summary>
        /// Sets the selection to the member associated with the given object.
        /// </summary>
        public void SetSelection(TValue assocMember) =>
            listInput.SetSelection(assocMember);

        /// <summary>
        /// Sets the selection to the specified entry.
        /// </summary>
        public void SetSelection(TElementContainer member) =>
            listInput.SetSelection(member);

        /// <summary>
        /// Clears the current selection
        /// </summary>
        public void ClearSelection() =>
            listInput.ClearSelection();

        protected virtual TElementContainer GetNewEntry()
        {
            var entry = new TElementContainer();
            entry.Element.Format = Format;
            entry.Element.Padding = _memberPadding;
            entry.Enabled = true;

            return entry;
        }

        protected virtual void ResetEntry(TElementContainer entry)
        {
            if (Selection == entry)
                listInput.ClearSelection();

            entry.Element.TextBoard.Clear();
            entry.AssocMember = default(TValue);
            entry.Enabled = true;
        }

        protected override void HandleInput(Vector2 cursorPos)
        {
            highlightBox.Visible = false;
            selectionBox.Visible = false;

            if (hudChain.Count > 0)
            {
                UpdateSelection();
            }

            listInput.ListSize = ListSize;
            listInput.ListPos = ListPos;
            listInput.ListRange = ListRange;
        }

        /// <summary>
        /// Update indices for selections, highlight and focus
        /// </summary>
        protected virtual void UpdateSelection()
        {
            UpdateSelectionPositions();
            UpdateSelectionFormatting();
        }

        protected virtual void UpdateSelectionPositions()
        {
            // Make sure the selection box highlights the current selection
            if (Selection != null && Selection.Element.Visible)
            {
                selectionBox.Offset = Selection.Element.Position - selectionBox.Origin;
                selectionBox.Size = Selection.Element.Size - HighlightPadding;
                selectionBox.Visible = Selection.Element.Visible;
            }

            // If highlight and selection indices dont match, draw highlight box
            if (listInput.HighlightIndex != listInput.SelectionIndex)
            {
                TElementContainer entry = hudChain[listInput.HighlightIndex];

                highlightBox.Visible = (listInput.IsMousedOver || listInput.HasFocus) && entry.Element.Visible;
                highlightBox.Size = entry.Element.Size - HighlightPadding;
                highlightBox.Offset = entry.Element.Position - highlightBox.Origin;
            }
        }

        protected virtual void UpdateSelectionFormatting()
        {
            // Update Selection/Highlight Formatting
            for (int i = ListRange.X; i <= ListRange.Y; i++)
                hudChain[i].Element.TextBoard.SetFormatting(Format);

            if ((SelectionIndex == listInput.FocusIndex) && SelectionIndex != -1)
            {
                if (
                    ( listInput.KeyboardScroll ^ (SelectionIndex != listInput.HighlightIndex) ) ||
                    ( !MouseInput.IsMousedOver && SelectionIndex == listInput.HighlightIndex )
                )
                {
                    selectionBox.Color = FocusColor;
                    hudChain[SelectionIndex].Element.TextBoard.SetFormatting(FocusFormat);
                }
                else
                    selectionBox.Color = HighlightColor;

                highlightBox.Color = HighlightColor;
            }
            else
            {
                if (listInput.KeyboardScroll)
                {
                    highlightBox.Color = FocusColor;
                    hudChain[listInput.HighlightIndex].Element.TextBoard.SetFormatting(FocusFormat);
                }
                else
                    highlightBox.Color = HighlightColor;

                selectionBox.Color = HighlightColor;
            }
        }

        protected override void Draw()
        {
            Size = hudChain.Size + Padding;
        }

        public virtual object GetOrSetMember(object data, int memberEnum)
        {
            var member = (ListBoxAccessors)memberEnum;

            switch (member)
            {
                case ListBoxAccessors.ListMembers:
                    return new CollectionData
                    (
                        x => hudChain.Collection[x].GetOrSetMember,
                        () => hudChain.Collection.Count
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
            public bool CanDrawTab { get; set; }

            public Color TabColor { get { return tabBoard.Color; } set { tabBoard.Color = value; } }

            private readonly MatBoard tabBoard;

            public HighlightBox(HudParentBase parent = null) : base(parent)
            {
                tabBoard = new MatBoard() { Color = TerminalFormatting.Mercury };
                Color = TerminalFormatting.Atomic;
                CanDrawTab = true;
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

                if (CanDrawTab && tabBoard.Color.A > 0)
                    tabBoard.Draw(tabPos, ref ptw);
            }
        }
    }
}
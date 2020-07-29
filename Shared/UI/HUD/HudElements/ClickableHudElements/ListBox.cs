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
        /// MyTuple<IList<RichStringMembers>, T>
        /// </summary>
        Add = 2,

        /// <summary>
        /// ApiMemberAccessor
        /// </summary>
        Selection = 3,
    }

    /// <summary>
    /// Scrollable list of text elements. Each list entry is associated with a value of type T.
    /// </summary>
    public class ListBox<T> : HudElementBase, IEnumerable<MyTuple<ListBoxEntry<T>, bool>>
    {
        /// <summary>
        /// Invoked when an entry is selected.
        /// </summary>
        public event Action<ListBox<T>> OnSelectionChanged;

        /// <summary>
        /// Used to allow the addition of list entries using collection-initializer syntax in
        /// conjunction with normal initializers.
        /// </summary>
        public ListBox<T> ListContainer => this;

        /// <summary>
        /// Read-only collection of list entries.
        /// </summary>
        public IReadOnlyList<MyTuple<ListBoxEntry<T>, bool>> ListEntries => scrollBox.ChainElements;

        /// <summary>
        /// Width of the list box in pixels.
        /// </summary>
        public override float Width { get { return scrollBox.Width; } set { scrollBox.Width = value; } }

        /// <summary>
        /// Height of the list box in pixels.
        /// </summary>
        public override float Height { get { return scrollBox.Height; } set { scrollBox.Height = value; } }

        /// <summary>
        /// Border size. Included in total element size.
        /// </summary>
        public override Vector2 Padding { get { return scrollBox.Padding; } set { scrollBox.Padding = value; } }

        /// <summary>
        /// Background color
        /// </summary>
        public Color Color { get { return scrollBox.Color; } set { scrollBox.Color = value; } }

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

                for (int n = 0; n < scrollBox.ChainElements.Count; n++)
                    scrollBox.ChainElements[n].Item1.Padding = value;
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
        public ListBoxEntry<T> Selection { get; private set; }

        protected readonly ScrollBox<ListBoxEntry<T>> scrollBox;
        protected readonly HighlightBox selectionBox, highlight;
        protected readonly BorderBox border;
        private Vector2 _memberPadding;

        private readonly ObjectPool<ListBoxEntry<T>> entryPool;

        public ListBox(IHudParent parent = null) : base(parent)
        {
            entryPool = new ObjectPool<ListBoxEntry<T>>(GetNewEntry, ResetEntry);

            scrollBox = new ScrollBox<ListBoxEntry<T>>(true, this)
            {
                SizingMode = HudChainSizingModes.FitMembersBoth | HudChainSizingModes.FitChainOffAxis,
            };

            border = new BorderBox(scrollBox)
            {
                DimAlignment = DimAlignments.Both,
                Color = new Color(58, 68, 77),
                Thickness = 1f,
            };

            selectionBox = new HighlightBox(scrollBox);
            highlight = new HighlightBox(scrollBox);

            Format = GlyphFormat.White;
            Padding = new Vector2(20f);
            Size = new Vector2(355f, 223f);

            HighlightPadding = new Vector2(12f, 6f);
            MemberPadding = new Vector2(20f, 6f);
            LineHeight = 30f;

            CaptureCursor = true;
        }

        private ListBoxEntry<T> GetNewEntry()
        {
            return new ListBoxEntry<T>()
            {
                Format = Format,
                Padding = _memberPadding,
            };
        }

        private void ResetEntry(ListBoxEntry<T> entry)
        {
            entry.MouseInput.ClearSubscribers();
            entry.AssocMember = default(T);
        }

        public IEnumerator<MyTuple<ListBoxEntry<T>, bool>> GetEnumerator() =>
            scrollBox.ChainElements.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();

        /// <summary>
        /// Adds a new member to the list box with the given name and associated
        /// object.
        /// </summary>
        public ListBoxEntry<T> Add(RichText name, T assocMember)
        {
            ListBoxEntry<T> member = entryPool.Get();

            member.TextBoard.SetText(name);
            member.AssocMember = assocMember;
            scrollBox.Add(member);

            return member;
        }

        /// <summary>
        /// Removes the given member from the list box.
        /// </summary>
        public void Remove(ListBoxEntry<T> member)
        {
            scrollBox.Remove(member);
            entryPool.Return(member);
        }

        /// <summary>
        /// Sets the selection to the member associated with the given object.
        /// </summary>
        public void SetSelection(int index)
        {
            if (index > 0 && index < scrollBox.ChainElements.Count)
            {
                Selection = scrollBox.ChainElements[index].Item1;
                scrollBox.SetElementEnabled(index, true);
                OnSelectionChanged?.Invoke(this);
            }
        }

        /// <summary>
        /// Sets the selection to the member associated with the given object.
        /// </summary>
        public void SetSelection(T assocMember)
        {
            int index = scrollBox.FindIndex(x => assocMember.Equals(x.AssocMember));

            if (index != -1)
            {
                Selection = scrollBox.ChainElements[index].Item1;
                scrollBox.SetElementEnabled(index, true);
                OnSelectionChanged?.Invoke(this);
            }
        }

        /// <summary>
        /// Sets the selection to the specified entry.
        /// </summary>
        public void SetSelection(ListBoxEntry<T> member)
        {
            int index = scrollBox.FindIndex(x => member.Equals(x));

            if (index != -1)
            {
                Selection = scrollBox.ChainElements[index].Item1;
                scrollBox.SetElementEnabled(index, true);
                OnSelectionChanged?.Invoke(this);
            }
        }

        protected override void Layout()
        {
            // Make sure the selection box highlights the current selection
            if (Selection != null)
            {
                selectionBox.Offset = Selection.Offset;
                selectionBox.Size = Selection.Size;
                selectionBox.Visible = Selection.Visible;
            }
            else
                selectionBox.Visible = false;
        }

        protected override void HandleInput()
        {
            highlight.Visible = false;

            for (int n = 0; n < scrollBox.ChainElements.Count; n++)
            {
                ListBoxEntry<T> entry = scrollBox.ChainElements[n].Item1;

                if (entry.IsMousedOver)
                {
                    highlight.Visible = true;
                    highlight.Size = entry.Size;
                    highlight.Offset = entry.Offset;

                    if (entry.MouseInput.IsLeftClicked)
                    {
                        Selection = entry;
                        OnSelectionChanged?.Invoke(this);
                    }
                }
            }
        }

        public new object GetOrSetMember(object data, int memberEnum)
        {
            var member = (ListBoxAccessors)memberEnum;

            switch (member)
            {
                case ListBoxAccessors.ListMembers:
                    return new CollectionData
                    (
                        x => scrollBox.ChainElements[x].Item1.GetOrSetMember, 
                        () => scrollBox.ChainElements.Count
                     );
                case ListBoxAccessors.Add:
                    {
                        var entryData = (MyTuple<IList<RichStringMembers>, T>)data;

                        return (ApiMemberAccessor)Add(new RichText(entryData.Item1), entryData.Item2).GetOrSetMember;
                    }
                case ListBoxAccessors.Selection:
                    {
                        if (data == null)
                            return (ApiMemberAccessor)Selection.GetOrSetMember;
                        else
                            SetSelection(data as ListBoxEntry<T>);

                        break;
                    }
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

            public HighlightBox(IHudParent parent = null) : base(parent)
            {
                tabBoard = new MatBoard() { Color = new Color(223, 230, 236) };
                Color = Color = new Color(34, 44, 53);
                ZOffset = HudLayers.Background;
            }

            protected override void Layout()
            {
                hudBoard.Size = cachedSize - cachedPadding;
                tabBoard.Size = new Vector2(4f * _scale, cachedSize.Y - cachedPadding.Y);
            }

            protected override void Draw(ref MatrixD matrix)
            {
                if (hudBoard.Color.A > 0)
                    hudBoard.Draw(cachedPosition, ref matrix);

                // Left align the tab
                Vector2 tabPos = cachedPosition;
                tabPos.X += (-hudBoard.Size.X + tabBoard.Size.X) / 2f;
                
                if (tabBoard.Color.A > 0)
                    tabBoard.Draw(tabPos, ref matrix);
            }
        }
    }
}
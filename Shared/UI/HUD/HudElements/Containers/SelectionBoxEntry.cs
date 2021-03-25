using System;
using System.Text;
using VRage;
using System.Collections.Generic;
using GlyphFormatMembers = VRage.MyTuple<byte, float, VRageMath.Vector2I, VRageMath.Color>;
using ApiMemberAccessor = System.Func<object, int, object>;

namespace RichHudFramework.UI
{
    public interface ISelectionBoxEntry<TElement> : IScrollBoxEntry<TElement>
    where TElement : HudElementBase
    {
        bool AllowHighlighting { get; set; }
    }

    public interface ISelectionBoxEntryTuple<TElement, TValue>
        : ISelectionBoxEntry<TElement>, IScrollBoxEntryTuple<TElement, TValue>
        where TElement : HudElementBase
    { }

    public class SelectionBoxEntry<TElement> : HudElementContainer<TElement>, ISelectionBoxEntry<TElement>
        where TElement : HudElementBase
    {
        public virtual bool Enabled { get; set; }

        public virtual bool AllowHighlighting { get; set; }

        public SelectionBoxEntry()
        {
            Enabled = true;
            AllowHighlighting = true;
        }
    }

    public class SelectionBoxEntryTuple<TElement, TValue>
        : SelectionBoxEntry<TElement>, ISelectionBoxEntryTuple<TElement, TValue>
        where TElement : HudElementBase
    {
        public TValue AssocMember { get; set; }
    }

}

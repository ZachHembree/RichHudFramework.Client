using System;
using System.Text;
using VRage;
using System.Collections.Generic;
using GlyphFormatMembers = VRage.MyTuple<byte, float, VRageMath.Vector2I, VRageMath.Color>;
using ApiMemberAccessor = System.Func<object, int, object>;

namespace RichHudFramework.UI
{
    using RichStringMembers = MyTuple<StringBuilder, GlyphFormatMembers>;

    public enum ListBoxEntryAccessors : int
    {
        /// <summary>
        /// IList<RichStringMembers>
        /// </summary>
        Name = 1,

        /// <summary>
        /// bool
        /// </summary>
        Enabled = 2,

        /// <summary>
        /// Object
        /// </summary>
        AssocObject = 3,

        /// <summary>
        /// Object
        /// </summary>
        ID = 4,
    }

    /// <summary>
    /// Interface implemented by objects that function as list box entries.
    /// </summary>
    public interface IListBoxEntry<TElement, TValue>
        : IScrollBoxEntryTuple<TElement, TValue>
        where TElement : HudElementBase, ILabelElement
    {
        object GetOrSetMember(object data, int memberEnum);
    }

    /// <summary>
    /// Label button assocated with an object of type T. Used in conjunction with list boxes.
    /// </summary>
    public class ListBoxEntry<TValue> : ListBoxEntry<TValue, LabelButton>
    { }

    public class ListBoxLabel<TValue> : ListBoxEntry<TValue, Label>
    { }

    public class ListBoxEntry<TValue, TElement>
        : ScrollBoxEntryTuple<TElement, TValue>, IListBoxEntry<TElement, TValue>
        where TElement : HudElementBase, ILabelElement, new()
    {
        public ListBoxEntry()
        {
            SetElement(new TElement() { AutoResize = false });
            Element.ZOffset = 1;
        }

        public object GetOrSetMember(object data, int memberEnum)
        {
            var member = (ListBoxEntryAccessors)memberEnum;

            switch (member)
            {
                case ListBoxEntryAccessors.Name:
                    {
                        if (data != null)
                            Element.Text = new RichText(data as List<RichStringMembers>);
                        else
                            return Element.Text.apiData;

                        break;
                    }
                case ListBoxEntryAccessors.Enabled:
                    {
                        if (data != null)
                            Enabled = (bool)data;
                        else
                            return Enabled;

                        break;
                    }
                case ListBoxEntryAccessors.AssocObject:
                    {
                        if (data != null)
                            AssocMember = (TValue)data;
                        else
                            return AssocMember;

                        break;
                    }
                case ListBoxEntryAccessors.ID:
                        return this;
            }

            return null;
        }
    }
}
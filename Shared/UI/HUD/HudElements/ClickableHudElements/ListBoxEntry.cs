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
        /// Object
        /// </summary>
        AssocObject = 3,

        /// <summary>
        /// Object
        /// </summary>
        ID = 4,
    }

    /// <summary>
    /// Label button assocated with an object of type T. Used in conjunction with list boxes.
    /// </summary>
    public class ListBoxEntry<T> : LabelButton
    {
        /// <summary>
        /// Object associated with the entry
        /// </summary>
        public T AssocMember { get; set; }

        public ListBoxEntry(T assocMember = default(T), IHudParent parent = null) : base(parent)
        {
            this.AssocMember = assocMember;
            AutoResize = false;
        }

        public new object GetOrSetMember(object data, int memberEnum)
        {
            var member = (ListBoxEntryAccessors)memberEnum;

            switch (member)
            {
                case ListBoxEntryAccessors.Name:
                    {
                        if (data == null)
                            TextBoard.SetText(new RichText(data as IList<RichStringMembers>));
                        else
                            return TextBoard.GetText().ApiData;

                        break;
                    }
                case ListBoxEntryAccessors.AssocObject:
                    {
                        if (data == null)
                            AssocMember = (T)data;
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
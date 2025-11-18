using System.Text;
using VRage;
using GlyphFormatMembers = VRage.MyTuple<byte, float, VRageMath.Vector2I, VRageMath.Color>;

namespace RichHudFramework.UI
{
    /// <summary>
    /// Interface implemented by objects that function as list box entries.
    /// </summary>
    public interface IListBoxEntry<TElement, TValue>
        : ISelectionBoxEntryTuple<TElement, TValue>
        where TElement : HudElementBase, IMinLabelElement
    {
		/// <summary>
		/// API interop method used by the Rich HUD Terminal
		/// </summary>
		/// <exclude/>
		object GetOrSetMember(object data, int memberEnum);
    }
}
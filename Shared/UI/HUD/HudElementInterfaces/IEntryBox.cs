using System.Text;
using VRage;
using System.Collections.Generic;
using GlyphFormatMembers = VRage.MyTuple<byte, float, VRageMath.Vector2I, VRageMath.Color>;

namespace RichHudFramework.UI
{
    /// <summary>
    /// An interface for clickable UI elements that represent of ListBoxEntry elements.
    /// </summary>
    public interface IEntryBox<TValue, TElementContainer, TElement> : IEnumerable<TElementContainer>, IReadOnlyHudElement
        where TElementContainer : IListBoxEntry<TElement, TValue>, new()
        where TElement : HudElementBase, IClickableElement, ILabelElement
    {
        /// <summary>
        /// Invoked when a member of the list is selected.
        /// </summary>
        event EventHandler SelectionChanged;

        /// <summary>
        /// Read-only collection of list entries.
        /// </summary>
        IReadOnlyList<TElementContainer> ListEntries { get; }

        /// <summary>
        /// Current selection. Null if empty.
        /// </summary>
        TElementContainer Selection { get; }
    }

    public interface IEntryBox<TValue> : IEntryBox<TValue, ListBoxEntry<TValue>, LabelButton>
    { }
}
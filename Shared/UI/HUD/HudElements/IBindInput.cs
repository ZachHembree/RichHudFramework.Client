using System.Collections;
using System.Collections.Generic;

namespace RichHudFramework.UI
{
	/// <summary>
	/// Provides input events for a specific custom UI binding.
	/// </summary>
	public interface IBindEventProxy
	{
		/// <summary>
		/// Invoked immediately when the bound input is first pressed
		/// </summary>
		event EventHandler NewPressed;

		/// <summary>
		/// Invoked after the bound input has been held and pressed for at least 500ms
		/// </summary>
		event EventHandler PressedAndHeld;

		/// <summary>
		/// Invoked immediately after the bound input is released
		/// </summary>
		event EventHandler Released;
	}

	/// <summary>
	/// Defines a set of custom bind inputs for a UI element
	/// </summary>
	public interface IBindInput : IFocusableElement, IEnumerable<IBindEventProxy>
	{
		/// <summary>
		/// Retrieves the event proxy (press/release events) for a specific bind on this UI element
		/// </summary>
		IBindEventProxy this[IBind bind] { get; }

		/// <summary>
		/// Adds a new bind to the input element if it hasn't been added before, and/or 
		/// registers the given event handlers to it.
		/// </summary>
		void Add(IBind bind, EventHandler NewPressed = null, EventHandler PressedAndHeld = null, EventHandler Released = null);

		/// <summary>
		/// Returns true if the given bind is actively used and handled by this element
		/// </summary>
		bool GetHasBind(IBind bind);
	}

	/// <summary>
	/// Represents a UI element that can respond to custom bind inputs
	/// </summary>
	public interface IBindInputElement : IFocusableElement
	{
		/// <summary>
		/// Custom bind input interface for this element
		/// </summary>
		IBindInput BindInput { get; }
	}
}
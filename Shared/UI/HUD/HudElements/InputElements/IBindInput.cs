using System.Collections.Generic;

namespace RichHudFramework.UI
{
	/// <summary>
	/// Provides per-bind event proxies (NewPressed / PressedAndHeld / Released) for custom control bindings
	/// attached to a UI element.
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
	/// Allows a UI element to respond to arbitrary custom control binds (<see cref="IBind"/>).
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
	/// Marks a UI element as supporting custom bind input via an <see cref="IBindInput"/> instance.
	/// </summary>
	public interface IBindInputElement : IFocusableElement
	{
		/// <summary>
		/// Custom bind input interface for this element
		/// </summary>
		IBindInput BindInput { get; }
	}
}
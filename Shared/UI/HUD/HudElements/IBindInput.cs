namespace RichHudFramework.UI
{
	public interface IBindEventProxy
	{
		/// <summary>
		/// Invoked when the bind is first pressed.
		/// </summary>
		event EventHandler NewPressed;

		/// <summary>
		/// Invoked after the bind has been held and pressed for at least 500ms.
		/// </summary>
		event EventHandler PressedAndHeld;

		/// <summary>
		/// Invoked after the bind has been released.
		/// </summary>
		event EventHandler Released;
	}

	/// <summary>
	/// Interface for invoking bind input events tied to a specific UI element
	/// </summary>
	public interface IBindInput
	{
		/// <summary>
		/// Owner of the bind input element that is sent in event callbacks
		/// </summary>
		IBindInputElement InputOwner { get; }

		/// <summary>
		/// Retrieves event group assocated with the given bind object for this 
		/// UI node
		/// </summary>
		IBindEventProxy this[IBind bind] { get; }

		/// <summary>
		/// Returns true if the given bind is used by the element
		/// </summary>
		bool GetHasBind(IBind bind);
	}

	public interface IBindInputElement : IReadOnlyHudElement
	{
		/// <summary>
		/// Bind input interface for the element
		/// </summary>
		IBindInput BindInput { get; }
	}
}
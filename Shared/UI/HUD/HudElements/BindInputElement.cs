using System;
using VRageMath;

namespace RichHudFramework.UI
{
	using Client;
	using System.Collections.Generic;

	/// <summary>
	/// UI element for invoking bind input events tied to a specific UI element
	/// </summary>
	public class BindInputElement : HudNodeBase, IBindInput
	{
		/// <summary>
		/// Owner of the bind input element that is sent in event callbacks
		/// </summary>
		public IBindInputElement InputOwner { get; set; }

		/// <summary>
		/// Retrieves event group assocated with the given bind object for this 
		/// UI node
		/// </summary>
		public IBindEventProxy this[IBind bind] => binds[bind];

		protected readonly Dictionary<IBind, BindEventProxy> binds;

		public BindInputElement(HudParentBase parent = null, IBindInputElement inputOwner = null) : base(parent)
		{
			InputOwner = inputOwner ?? parent as IBindInputElement;
		}

		/// <summary>
		/// Adds a new bind to the input element if it hasn't been added before
		/// </summary>
		public void Add(IBind bind)
		{
			if (!binds.ContainsKey(bind))
				binds.Add(bind, new BindEventProxy());
		}

		/// <summary>
		/// Removes all binds from the element
		/// </summary>
		public void Reset()
		{
			binds.Clear();
		}

		/// <summary>
		/// Unregisters all subscribers from all bind events
		/// </summary>
		public void ClearSubscribers()
		{
			foreach (var pair in binds)
				pair.Value.ClearSubscribers();
		}

		/// <summary>
		/// Returns true if the given bind is used by the element
		/// </summary>
		public bool GetHasBind(IBind bind) =>
			binds.ContainsKey(bind);

		protected override void HandleInput(Vector2 cursorPos)
		{
			foreach (var pair in binds)
			{
				if (pair.Key.IsNewPressed)
				{
					pair.Value.InvokeNewPressed(InputOwner, EventArgs.Empty);
				}

				if (pair.Key.IsPressedAndHeld)
				{
					pair.Value.InvokePressedAndHeld(InputOwner, EventArgs.Empty);
				}

				if (pair.Key.IsReleased)
				{
					pair.Value.InvokeReleased(InputOwner, EventArgs.Empty);
				}
			}
		}

		protected class BindEventProxy : IBindEventProxy
		{
			/// <summary>
			/// Invoked when the bind is first pressed.
			/// </summary>
			public event EventHandler NewPressed;

			/// <summary>
			/// Invoked after the bind has been held and pressed for at least 500ms.
			/// </summary>
			public event EventHandler PressedAndHeld;

			/// <summary>
			/// Invoked after the bind has been released.
			/// </summary>
			public event EventHandler Released;

			public void InvokeNewPressed(object sender, EventArgs args) =>
				NewPressed?.Invoke(sender, args);

			public void InvokePressedAndHeld(object sender, EventArgs args) =>
				PressedAndHeld?.Invoke(sender, args);

			public void InvokeReleased(object sender, EventArgs args) =>
				Released?.Invoke(sender, args);

			public void ClearSubscribers()
			{
				NewPressed = null;
				PressedAndHeld = null;
				Released = null;
			}
		}
	}
}
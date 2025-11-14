using System;
using VRageMath;

namespace RichHudFramework.UI
{
	using Client;
	using System.Collections;
	using System.Collections.Generic;

	/// <summary>
	/// Defines a set of custom bind inputs for a UI element
	/// </summary>
	public class BindInputElement : HudNodeBase, IBindInput
	{
		/// <summary>
		/// Allows the addition of binds in conjunction with normal property initialization
		/// </summary>
		public IBindInput CollectionInitializer => this;

		/// <summary>
		/// Element that owns this input, used for event callbacks
		/// </summary>
		public IFocusHandler FocusHandler { get; protected set; }

		/// <summary>
		/// Retrieves the event proxy (press/release events) for a specific bind on this UI element
		/// </summary>
		public IBindEventProxy this[IBind bind] => binds[bind];

		/// <summary>
		/// If true, then input events will only fire if the element has input focus.
		/// False by default.
		/// </summary>
		public bool IsFocusRequired { get; set; }

		protected readonly Dictionary<IBind, BindEventProxy> binds;

		public BindInputElement(HudParentBase parent = null) : base(parent)
		{
			FocusHandler = (parent as IFocusableElement)?.FocusHandler;
			IsFocusRequired = false;
			binds = new Dictionary<IBind, BindEventProxy>();
		}

		/// <summary>
		/// Adds a new bind to the input element if it hasn't been added before, and/or 
		/// registers the given event handlers to it.
		/// </summary>
		public void Add(IBind bind, EventHandler NewPressed = null, EventHandler PressedAndHeld = null, EventHandler Released = null)
		{
			if (!binds.ContainsKey(bind))
				binds.Add(bind, new BindEventProxy());

			if (NewPressed != null || PressedAndHeld != null | Released != null)
			{
				var proxy = binds[bind];

				if (NewPressed != null)
					proxy.NewPressed += NewPressed;

				if (PressedAndHeld != null)
					proxy.PressedAndHeld += PressedAndHeld;

				if (Released != null)
					proxy.Released += Released;
			}
		}

		/// <summary>
		/// Removes all binds from the element
		/// </summary>
		public void Reset() { binds.Clear(); }

		/// <summary>
		/// Returns true if the given bind is actively used and handled by this element
		/// </summary>
		public bool GetHasBind(IBind bind) =>
			binds.ContainsKey(bind);

		protected override void HandleInput(Vector2 cursorPos)
		{
			FocusHandler = (Parent as IFocusableElement)?.FocusHandler;

			if (IsFocusRequired && !(FocusHandler?.HasFocus ?? false))
				return;

			foreach (KeyValuePair<IBind, BindEventProxy> pair in binds)
			{
				if (pair.Key.IsNewPressed)
					pair.Value.InvokeNewPressed(FocusHandler, EventArgs.Empty);

				if (pair.Key.IsPressedAndHeld)
					pair.Value.InvokePressedAndHeld(FocusHandler, EventArgs.Empty);

				if (pair.Key.IsReleased)
					pair.Value.InvokeReleased(FocusHandler, EventArgs.Empty);
			}
		}

		public IEnumerator<IBindEventProxy> GetEnumerator() =>
			binds.Values.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		/// <summary>
		/// Provides input events for a specific custom UI binding.
		/// </summary>
		protected class BindEventProxy : IBindEventProxy
		{
			/// <summary>
			/// Invoked immediately when the bound input is first pressed
			/// </summary>
			public event EventHandler NewPressed;

			/// <summary>
			/// Invoked after the bound input has been held and pressed for at least 500ms
			/// </summary>
			public event EventHandler PressedAndHeld;

			/// <summary>
			/// Invoked immediately after the bound input is released
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
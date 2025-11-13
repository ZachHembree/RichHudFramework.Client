using System;

namespace RichHudFramework
{
	namespace UI
	{
		using Client;

		/// <summary>
		/// Handles input focus for a UI node
		/// </summary>
		public class InputFocusHandler : IFocusHandler
		{
			/// <summary>
			/// Owner of the input that is sent in EventHandler invocations
			/// </summary>
			public IFocusableElement InputOwner { get; set; }

			/// <summary>
			/// Indicates whether the UI element has input focus
			/// </summary>
			public bool HasFocus { get; private set; }

			/// <summary>
			/// Invoked when taking focus
			/// </summary>
			public event EventHandler GainedInputFocus;

			/// <summary>
			/// Invoked when focus is lost
			/// </summary>
			public event EventHandler LostInputFocus;

			public InputFocusHandler(IFocusableElement inputOwner)
			{
				InputOwner = inputOwner;
			}

			/// <summary>
			/// Gets input focus for controls. Input focus is normally taken automatically when an
			/// element with mouse input is clicked, but can be taken manually.
			/// </summary>
			public void GetInputFocus()
			{
				if (!HasFocus)
				{
					HudMain.GetInputFocus(this);
					HasFocus = true;
					GainedInputFocus?.Invoke(InputOwner, EventArgs.Empty);
				}
			}

			/// <summary>
			/// Releases input focus. Typically used for focus lost callback.
			/// </summary>
			public void ReleaseFocus()
			{
				if (!HasFocus)
				{
					HasFocus = false;
					LostInputFocus?.Invoke(InputOwner, EventArgs.Empty);
				}
			}
		}
	}
}
using System;
using VRage;

namespace RichHudFramework
{
    namespace UI
    {
		/// <summary>
		/// Defines properties and events for managing mouse input on a UI element.
		/// </summary>
		public interface IMouseInput : IFocusableElement
		{
			/// <summary>
			/// Invoked when the mouse cursor enters the element's interactive area.
			/// </summary>
			event EventHandler CursorEntered;

			/// <summary>
			/// Invoked when the mouse cursor leaves the element's interactive area.
			/// </summary>
			event EventHandler CursorExited;

			/// <summary>
			/// Invoked when the element is clicked with the left mouse button.
			/// </summary>
			event EventHandler LeftClicked;

			/// <summary>
			/// Invoked when the left mouse button is released over the element.
			/// </summary>
			event EventHandler LeftReleased;

			/// <summary>
			/// Invoked when the element is clicked with the right mouse button.
			/// </summary>
			event EventHandler RightClicked;

			/// <summary>
			/// Invoked when the right mouse button is released over the element.
			/// </summary>
			event EventHandler RightReleased;

			/// <summary>
			/// Optional tooltip text shown when the element is moused over.
			/// </summary>
			ToolTip ToolTip { get; set; }

			/// <summary>
			/// Returns true if the element is currently being held down with the left mouse button.
			/// </summary>
			bool IsLeftClicked { get; }

			/// <summary>
			/// Returns true if the element is currently being held down with the right mouse button.
			/// </summary>
			bool IsRightClicked { get; }

			/// <summary>
			/// Returns true if the element was just clicked with the left mouse button this frame.
			/// </summary>
			bool IsNewLeftClicked { get; }

			/// <summary>
			/// Returns true if the element was just clicked with the right mouse button this frame.
			/// </summary>
			bool IsNewRightClicked { get; }

			/// <summary>
			/// Returns true if the element was just released after being left-clicked this frame.
			/// </summary>
			bool IsLeftReleased { get; }

			/// <summary>
			/// Returns true if the element was just released after being right-clicked this frame.
			/// </summary>
			bool IsRightReleased { get; }

			/// <summary>
			/// Returns true if the mouse cursor is currently over the element.
			/// </summary>
			bool IsMousedOver { get; }
		}

		/// <summary>
		/// Represents a UI element that can be interacted with via the mouse
		/// </summary>
		public interface IClickableElement : IFocusableElement
		{
			/// <summary>
			/// Mouse input interface for this clickable element
			/// </summary>
			IMouseInput MouseInput { get; }
		}
	}
}
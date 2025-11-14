using System;
using VRageMath;

namespace RichHudFramework.UI
{
	using Client;
	using Server;
	using static NodeConfigIndices;

	/// <summary>
	/// A clickable box. Doesn't render any textures or text. Must be used in conjunction with other elements.
	/// Events return the parent object, or InputOwner if specified.
	/// </summary>
	public class MouseInputElement : HudElementBase, IMouseInput
	{
		/// <summary>
		/// Element that owns this input, used for event callbacks.
		/// </summary>
		public IFocusHandler FocusHandler { get; protected set; }

		/// <summary>
		/// Invoked when the mouse cursor enters the element's interactive area.
		/// </summary>
		public event EventHandler CursorEntered;

		/// <summary>
		/// Invoked when the mouse cursor leaves the element's interactive area.
		/// </summary>
		public event EventHandler CursorExited;

		/// <summary>
		/// Invoked when the element is clicked with the left mouse button.
		/// </summary>
		public event EventHandler LeftClicked;

		/// <summary>
		/// Invoked when the left mouse button is released over the element.
		/// </summary>
		public event EventHandler LeftReleased;

		/// <summary>
		/// Invoked when the element is clicked with the right mouse button.
		/// </summary>
		public event EventHandler RightClicked;

		/// <summary>
		/// Invoked when the right mouse button is released over the element.
		/// </summary>
		public event EventHandler RightReleased;

		/// <summary>
		/// Invoked when the mouse cursor enters the element's interactive area. Event initializer.
		/// </summary>
		public EventHandler CursorEnteredCallback { set { CursorEntered += value; } }

		/// <summary>
		/// Invoked when the mouse cursor leaves the element's interactive area. Event initializer.
		/// </summary>
		public EventHandler CursorExitedCallback { set { CursorExited += value; } }
		/// <summary>
		/// Invoked when the element is clicked with the left mouse button. Event initializer.
		/// </summary>
		public EventHandler LeftClickedCallback { set { LeftClicked += value; } }

		/// <summary>
		/// Invoked when the left mouse button is released over the element. Event initializer.
		/// </summary>
		public EventHandler LeftReleasedCallback { set { LeftReleased += value; } }

		/// <summary>
		/// Invoked when the element is clicked with the right mouse button. Event initializer.
		/// </summary>
		public EventHandler RightClickedCallback { set { RightClicked += value; } }

		/// <summary>
		/// Invoked when the right mouse button is released over the element. Event initializer.
		/// </summary>
		public EventHandler RightReleasedCallback { set { RightReleased += value; } }

		/// <summary>
		/// Optional tooltip text shown when the element is moused over.
		/// </summary>
		public ToolTip ToolTip { get; set; }

		/// <summary>
		/// Returns true if the element is currently being held down with the left mouse button.
		/// </summary>
		public bool IsLeftClicked { get; private set; }

		/// <summary>
		/// Returns true if the element is currently being held down with the right mouse button.
		/// </summary>
		public bool IsRightClicked { get; private set; }

		/// <summary>
		/// Returns true if the element was just clicked with the left mouse button this frame.
		/// </summary>
		public bool IsNewLeftClicked { get; private set; }

		/// <summary>
		/// Returns true if the element was just clicked with the right mouse button this frame.
		/// </summary>
		public bool IsNewRightClicked { get; private set; }

		/// <summary>
		/// Returns true if the element was just released after being left-clicked this frame.
		/// </summary>
		public bool IsLeftReleased { get; private set; }

		/// <summary>
		/// Returns true if the element was just released after being right-clicked this frame.
		/// </summary>
		public bool IsRightReleased { get; private set; }

		private bool mouseCursorEntered;

		public MouseInputElement(HudParentBase parent) : base(parent)
		{
			FocusHandler = (parent as IFocusableElement)?.FocusHandler;
			UseCursor = true;
			ShareCursor = true;
			DimAlignment = DimAlignments.UnpaddedSize;
		}

		public MouseInputElement() : this(null)
		{ }

		/// <summary>
		/// Clears all subscribers to mouse input events.
		/// </summary>
		public void ClearSubscribers()
		{
			CursorEntered = null;
			CursorExited = null;
			LeftClicked = null;
			LeftReleased = null;
			RightClicked = null;
			RightReleased = null;
		}

		protected override void InputDepth()
		{
			if (HudSpace.IsFacingCamera)
			{
				Vector3 cursorPos = HudSpace.CursorPos;
				Vector2 halfSize = Vector2.Max(CachedSize, new Vector2(minMouseBounds)) * .5f;
				BoundingBox2 box = new BoundingBox2(Position - halfSize, Position + halfSize);
				bool mouseInBounds;

				if (maskingBox == null)
				{
					mouseInBounds = box.Contains(new Vector2(cursorPos.X, cursorPos.Y)) == ContainmentType.Contains
						|| (IsLeftClicked || IsRightClicked);
				}
				else
				{
					mouseInBounds = box.Intersect(maskingBox.Value).Contains(new Vector2(cursorPos.X, cursorPos.Y)) == ContainmentType.Contains
						|| (IsLeftClicked || IsRightClicked);
				}

				if (mouseInBounds)
				{
					_config[StateID] |= (uint)HudElementStates.IsMouseInBounds;
					HudMain.Cursor.TryCaptureHudSpace(cursorPos.Z, HudSpace.GetHudSpaceFunc);
				}
			}
		}

		protected override void HandleInput(Vector2 cursorPos)
		{
			FocusHandler = (Parent as IFocusableElement)?.FocusHandler;

			if (IsMousedOver)
			{
				if (!mouseCursorEntered)
				{
					mouseCursorEntered = true;
					CursorEntered?.Invoke(FocusHandler?.InputOwner, EventArgs.Empty);
				}

				if (SharedBinds.LeftButton.IsNewPressed)
				{
					FocusHandler?.GetInputFocus();
					OnLeftClick();
				}
				else
					IsNewLeftClicked = false;

				if (SharedBinds.RightButton.IsNewPressed)
				{
					FocusHandler?.GetInputFocus();
					OnRightClick();
				}
				else
					IsNewRightClicked = false;

				if (ToolTip != null)
					HudMain.Cursor.RegisterToolTip(ToolTip);
			}
			else
			{
				if (mouseCursorEntered)
				{
					mouseCursorEntered = false;
					CursorExited?.Invoke(FocusHandler?.InputOwner, EventArgs.Empty);
				}

				bool hasFocus = FocusHandler?.HasFocus ?? false;

				if (hasFocus && (SharedBinds.LeftButton.IsNewPressed || SharedBinds.RightButton.IsNewPressed))
					FocusHandler.ReleaseFocus();

				IsNewLeftClicked = false;
				IsNewRightClicked = false;
			}

			if (!SharedBinds.LeftButton.IsPressed && IsLeftClicked)
			{
				LeftReleased?.Invoke(FocusHandler?.InputOwner, EventArgs.Empty);
				IsLeftReleased = true;
				IsLeftClicked = false;
			}
			else
				IsLeftReleased = false;

			if (!SharedBinds.RightButton.IsPressed && IsRightClicked)
			{
				RightReleased?.Invoke(FocusHandler?.InputOwner, EventArgs.Empty);
				IsRightReleased = true;
				IsRightClicked = false;
			}
			else
				IsRightReleased = false;
		}

		/// <summary>
		/// Invokes left click event
		/// </summary>
		public virtual void OnLeftClick()
		{
			LeftClicked?.Invoke(FocusHandler?.InputOwner, EventArgs.Empty);
			IsLeftClicked = true;
			IsNewLeftClicked = true;
			IsLeftReleased = false;
		}

		/// <summary>
		/// Invokes right click event
		/// </summary>
		public virtual void OnRightClick()
		{
			RightClicked?.Invoke(FocusHandler?.InputOwner, EventArgs.Empty);
			IsRightClicked = true;
			IsNewRightClicked = true;
			IsRightReleased = false;
		}
	}
}
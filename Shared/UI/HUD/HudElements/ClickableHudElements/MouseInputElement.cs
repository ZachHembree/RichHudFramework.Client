using System;
using VRageMath;

namespace RichHudFramework.UI
{
	using Client;
	using static NodeConfigIndices;

	/// <summary>
	/// A clickable box. Doesn't render any textures or text. Must be used in conjunction with other elements.
	/// Events return the parent object, or InputOwner if specified.
	/// </summary>
	public class MouseInputElement : HudElementBase, IMouseInput
	{
		/// <summary>
		/// Mouse input owner sent in EventHandler invocations
		/// </summary>
		public IClickableElement InputOwner { get; set; }

		/// <summary>
		/// Invoked when the cursor enters the element's bounds
		/// </summary>
		public event EventHandler CursorEntered;

		/// <summary>
		/// Invoked when the cursor leaves the element's bounds
		/// </summary>
		public event EventHandler CursorExited;

		/// <summary>
		/// Invoked when the element is clicked with the left mouse button
		/// </summary>
		public event EventHandler LeftClicked;

		/// <summary>
		/// Invoked when the left click is released
		/// </summary>
		public event EventHandler LeftReleased;

		/// <summary>
		/// Invoked when the element is clicked with the right mouse button
		/// </summary>
		public event EventHandler RightClicked;

		/// <summary>
		/// Invoked when the right click is released
		/// </summary>
		public event EventHandler RightReleased;

		/// <summary>
		/// Invoked when taking focus
		/// </summary>
		public event EventHandler GainedInputFocus;

		/// <summary>
		/// Invoked when focus is lost
		/// </summary>
		public event EventHandler LostInputFocus;

		/// <summary>
		/// Optional tooltip shown when the element is moused over
		/// </summary>
		public ToolTip ToolTip { get; set; }

		/// <summary>
		/// Indicates whether or not the element has input focus.
		/// </summary>
		public bool HasFocus { get { return hasFocus && Visible; } private set { hasFocus = value; } }

		/// <summary>
		/// True if the element is being clicked with the left mouse button
		/// </summary>
		public bool IsLeftClicked { get; private set; }

		/// <summary>
		/// True if the element is being clicked with the right mouse button
		/// </summary>
		public bool IsRightClicked { get; private set; }

		/// <summary>
		/// True if the element was just clicked with the left mouse button
		/// </summary>
		public bool IsNewLeftClicked { get; private set; }

		/// <summary>
		/// True if the element was just clicked with the right mouse button
		/// </summary>
		public bool IsNewRightClicked { get; private set; }

		/// <summary>
		/// True if the element was just released after being left clicked
		/// </summary>
		public bool IsLeftReleased { get; private set; }

		/// <summary>
		/// True if the element was just released after being right clicked
		/// </summary>
		public bool IsRightReleased { get; private set; }

		private bool mouseCursorEntered;
		private bool hasFocus;
		protected readonly Action LoseFocusCallback;

		public MouseInputElement(HudParentBase parent, IClickableElement inputOwner = null) : base(parent)
		{
			InputOwner = inputOwner ?? parent as IClickableElement;
			UseCursor = true;
			ShareCursor = true;
			HasFocus = false;
			DimAlignment = DimAlignments.UnpaddedSize;

			LoseFocusCallback = LoseFocus;
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
					Config[StateID] |= (uint)HudElementStates.IsMouseInBounds;
					HudMain.Cursor.TryCaptureHudSpace(cursorPos.Z, HudSpace.GetHudSpaceFunc);
				}
			}
		}

		protected override void HandleInput(Vector2 cursorPos)
		{
			if (IsMousedOver)
			{
				if (!mouseCursorEntered)
				{
					mouseCursorEntered = true;
					CursorEntered?.Invoke(InputOwner, EventArgs.Empty);
				}

				if (SharedBinds.LeftButton.IsNewPressed)
				{
					GetInputFocus();
					OnLeftClick();
				}
				else
					IsNewLeftClicked = false;

				if (SharedBinds.RightButton.IsNewPressed)
				{
					GetInputFocus();
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
					CursorExited?.Invoke(InputOwner, EventArgs.Empty);
				}

				if (HasFocus && (SharedBinds.LeftButton.IsNewPressed || SharedBinds.RightButton.IsNewPressed))
					LoseFocus();

				IsNewLeftClicked = false;
				IsNewRightClicked = false;
			}

			if (!SharedBinds.LeftButton.IsPressed && IsLeftClicked)
			{
				LeftReleased?.Invoke(InputOwner, EventArgs.Empty);
				IsLeftReleased = true;
				IsLeftClicked = false;
			}
			else
				IsLeftReleased = false;

			if (!SharedBinds.RightButton.IsPressed && IsRightClicked)
			{
				RightReleased?.Invoke(InputOwner, EventArgs.Empty);
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
			LeftClicked?.Invoke(InputOwner, EventArgs.Empty);
			IsLeftClicked = true;
			IsNewLeftClicked = true;
			IsLeftReleased = false;
		}

		/// <summary>
		/// Invokes right click event
		/// </summary>
		public virtual void OnRightClick()
		{
			RightClicked?.Invoke(InputOwner, EventArgs.Empty);
			IsRightClicked = true;
			IsNewRightClicked = true;
			IsRightReleased = false;
		}

		/// <summary>
		/// Gets input focus for keyboard controls. Input focus normally taken when an
		/// element with mouse input is clicked.
		/// </summary>
		public virtual void GetInputFocus()
		{
			if (!hasFocus)
			{
				hasFocus = true;
				HudMain.GetInputFocus(LoseFocusCallback);
				GainedInputFocus?.Invoke(InputOwner, EventArgs.Empty);
			}
		}

		protected virtual void LoseFocus()
		{
			if (hasFocus)
			{
				hasFocus = false;
				LostInputFocus?.Invoke(InputOwner, EventArgs.Empty);
			}
		}
	}
}
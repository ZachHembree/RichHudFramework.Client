using System;
using VRageMath;

namespace RichHudFramework.UI
{
    /// <summary>
    /// Bordered checkbox designed to mimic the appearance of the checkbox used in the SE terminal
    /// (sans name tag).
    /// </summary>
    public class BorderedCheckBox : Button
    {
		/// <summary>
		/// Invoked when the current value changes
		/// </summary>
		public event EventHandler ValueChanged;

		/// <summary>
		/// Registers a value update callback. Useful in initializers.
		/// </summary>
		public EventHandler UpdateValueCallback
		{
			set { ValueChanged += value; }
		}

		/// <summary>
		/// Indicates whether or not the box is checked.
		/// </summary>
		public bool IsBoxChecked { get; set; }

        /// <summary>
        /// Color of the border surrounding the button
        /// </summary>
        public Color BorderColor { get { return border.Color; } set { border.Color = value; } }

        /// <summary>
        /// Thickness of the border surrounding the button
        /// </summary>
        public float BorderThickness { get { return border.Thickness; } set { border.Thickness = value; } }

        /// <summary>
        /// Tickbox default color
        /// </summary>
        public Color TickBoxColor { get { return tickBox.Color; } set { tickBox.Color = value; } }

        /// <summary>
        /// Tickbox highlight color
        /// </summary>
        public Color TickBoxHighlightColor { get; set; }

        /// <summary>
        /// Tickbox focus color
        /// </summary>
        public Color TickBoxFocusColor { get; set; }

        /// <summary>
        /// Background color used when the control gains focus.
        /// </summary>
        public Color FocusColor { get; set; }

        /// <summary>
        /// If true, then the button will change formatting when it takes focus.
        /// </summary>
        public bool UseFocusFormatting { get; set; }

        protected readonly BorderBox border;
        protected readonly TexturedBox tickBox;
        protected Color lastTickColor;
        protected bool lastValue;

        public BorderedCheckBox(HudParentBase parent) : base(parent)
        {
            border = new BorderBox(this)
            {
                Thickness = 1f,
                DimAlignment = DimAlignments.Size,
            };

            tickBox = new TexturedBox(this)
            {
                DimAlignment = DimAlignments.UnpaddedSize,
                Padding = new Vector2(17f),
            };

            IsBoxChecked = true;
			Size = new Vector2(37f);

            Color = TerminalFormatting.OuterSpace;
            HighlightColor = TerminalFormatting.Atomic;
            FocusColor = TerminalFormatting.Mint;

            TickBoxColor = TerminalFormatting.StormGrey;
            TickBoxHighlightColor = Color.White;
            TickBoxFocusColor = TerminalFormatting.Cinder;

            BorderColor = TerminalFormatting.LimedSpruce;
            UseFocusFormatting = true;
            lastValue = IsBoxChecked;

            MouseInput.LeftClicked += ToggleValue;
            FocusHandler.GainedInputFocus += GainFocus;
			FocusHandler.LostInputFocus += LoseFocus;
        }

        public BorderedCheckBox() : this(null)
        { }

		protected override void HandleInput(Vector2 cursorPos)
        {
            tickBox.Visible = IsBoxChecked;

            if (FocusHandler.HasFocus)
            {
                if (SharedBinds.Space.IsNewPressed)
                {
                    _mouseInput.OnLeftClick();
                }
            }

            if (lastValue != IsBoxChecked)
            {
                ValueChanged?.Invoke(FocusHandler?.InputOwner, EventArgs.Empty);
                lastValue = IsBoxChecked;
            }
        }

        private void ToggleValue(object sender, EventArgs args)
        {
            IsBoxChecked = !IsBoxChecked;
        }

        protected override void CursorEnter(object sender, EventArgs args)
        {
            if (HighlightEnabled)
            {
                if (!(UseFocusFormatting && FocusHandler.HasFocus))
                {
                    lastBackgroundColor = Color;
                    lastTickColor = TickBoxColor;
                }

                Color = HighlightColor;
                TickBoxColor = TickBoxHighlightColor;
            }
        }

        protected override void CursorExit(object sender, EventArgs args)
        {
            if (HighlightEnabled)
            {
                if (UseFocusFormatting && FocusHandler.HasFocus)
                {
                    Color = FocusColor;
                    TickBoxColor = TickBoxFocusColor;
                }
                else
                {
                    Color = lastBackgroundColor;
                    TickBoxColor = lastTickColor;
                }
            }
        }

        protected virtual void GainFocus(object sender, EventArgs args)
        {
            if (HighlightEnabled)
            {
                if (UseFocusFormatting && !MouseInput.IsMousedOver)
                {
                    Color = FocusColor;
                    TickBoxColor = TickBoxFocusColor;
                }
            }
        }

        protected virtual void LoseFocus(object sender, EventArgs args)
        {
            if (HighlightEnabled)
            {
                if (UseFocusFormatting)
                {
                    Color = lastBackgroundColor;
                    TickBoxColor = lastTickColor;
                }
            }
        }
    }
}
using System;
using System.Text;
using RichHudFramework.UI.Rendering;
using VRageMath;

namespace RichHudFramework.UI
{
    using UI;

    /// <summary>
    /// Named color picker using sliders designed to mimic the appearance of the color picker in the SE terminal.
    /// RGB only. Alpha not supported.
    /// </summary>
    public class ColorPickerRGB : HudElementBase
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
		/// Text rendered by the label
		/// </summary>
		public RichText Name { get { return name.TextBoard.GetText(); } set { name.TextBoard.SetText(value); } }

        /// <summary>
        /// Text builder backing the label
        /// </summary>
        public ITextBuilder NameBuilder => name.TextBoard;

        /// <summary>
        /// Formatting used by the label
        /// </summary>
        public GlyphFormat NameFormat { get { return name.TextBoard.Format; } set { name.TextBoard.SetFormatting(value); } }

        /// <summary>
        /// Formatting used by the color value labels
        /// </summary>
        public GlyphFormat ValueFormat
        { 
            get { return sliderText[0].Format; } 
            set 
            {
                foreach (Label label in sliderText)
                    label.TextBoard.SetFormatting(value);
            } 
        }

        /// <summary>
        /// Color currently specified by the color picker
        /// </summary>
        public Color Color 
        { 
            get { return _color; }
            set 
            {
                sliders[0].Current = value.R;
                sliders[1].Current = value.G;
                sliders[2].Current = value.B;
                _color = value;
            }
        }

        // Header
        protected readonly Label name;
        protected readonly TexturedBox display;
        protected readonly HudChain headerChain;
        // Slider text
        protected readonly Label[] sliderText;
        protected readonly HudChain<HudElementContainer<Label>, Label> colorNameColumn;
        // Sliders
        public readonly SliderBox[] sliders;
        protected readonly HudChain<HudElementContainer<SliderBox>, SliderBox> colorSliderColumn;

        protected readonly HudChain colorChain;
        protected Color _color, lastColor;
        protected int focusedChannel;

        public ColorPickerRGB(HudParentBase parent) : base(parent)
        {
            // Header
            name = new Label()
            {
                Format = GlyphFormat.Blueish.WithSize(1.08f),
                Text = "NewColorPicker",
                AutoResize = false,
                Size = new Vector2(88f, 22f)
            };

            display = new TexturedBox()
            {
                Width = 231f,
                Color = Color.Black
            };

            var dispBorder = new BorderBox(display)
            {
                Color = Color.White,
                Thickness = 1f,
                DimAlignment = DimAlignments.Size,
            };

            headerChain = new HudChain(false)
            {
                Height = 22f,
                SizingMode = HudChainSizingModes.FitMembersOffAxis,
                CollectionContainer = { name, { display, 1f } }
            };

            // Color picker
            sliderText = new Label[]
            {
                new Label() { AutoResize = false, Format = TerminalFormatting.ControlFormat, Height = 47f },
                new Label() { AutoResize = false, Format = TerminalFormatting.ControlFormat, Height = 47f },
                new Label() { AutoResize = false, Format = TerminalFormatting.ControlFormat, Height = 47f }
            };

            colorNameColumn = new HudChain<HudElementContainer<Label>, Label>(true)
            {
                SizingMode = HudChainSizingModes.FitMembersOffAxis,
                Width = 87f,
                Spacing = 5f,
                CollectionContainer =
                {
                    { sliderText[0], 1f },
                    { sliderText[1], 1f }, 
                    { sliderText[2], 1f } 
                }
            };

            sliders = new SliderBox[] 
            {
                new SliderBox() 
                { 
                    Min = 0f, Max = 255f, Height = 47f, 
                    UpdateValueCallback = UpdateChannelR
                },
                new SliderBox() 
                { 
                    Min = 0f, Max = 255f, Height = 47f,
					UpdateValueCallback = UpdateChannelG
				},
                new SliderBox() 
                { 
                    Min = 0f, Max = 255f, Height = 47f,
					UpdateValueCallback = UpdateChannelB
				}
            };

            colorSliderColumn = new HudChain<HudElementContainer<SliderBox>, SliderBox>(true)
            {
                SizingMode = HudChainSizingModes.FitMembersOffAxis,
                Width = 231f,
                Spacing = 5f,
                CollectionContainer = 
                { 
                    { sliders[0], 1f }, 
                    { sliders[1], 1f }, 
                    { sliders[2], 1f } 
                }
            };

            colorChain = new HudChain(false)
            {
                SizingMode = HudChainSizingModes.FitMembersOffAxis,
                CollectionContainer = { { colorNameColumn, 0f }, { colorSliderColumn, 1f } }
            };

            var mainChain = new HudChain(true, this)
            {
                DimAlignment = DimAlignments.UnpaddedSize,
                SizingMode = HudChainSizingModes.FitMembersOffAxis,
                Spacing = 5f,
                CollectionContainer =
                {
                    { headerChain, 0f },
                    { colorChain, 1f },
                }
            };

            Size = new Vector2(318f, 163f);
            UseCursor = true;
            ShareCursor = true;
            focusedChannel = -1;
            Color = Color.White;
            lastColor = _color;
        }

        public ColorPickerRGB() : this(null)
        { }

        /// <summary>
        /// Set focus for slider corresponding to the given color channel index [0, 2].
        /// </summary>
        public void SetChannelFocused(int channel)
        {
            channel = MathHelper.Clamp(channel, 0, 2);

            if (!sliders[channel].FocusHandler.HasFocus)
                focusedChannel = channel;
        }

        protected virtual void UpdateChannelR(object sender, EventArgs args)
        {
			var slider = sender as SliderBox;
			_color.R = (byte)Math.Round(slider.Current);
			sliderText[0].TextBoard.SetText($"R: {_color.R}");
			display.Color = _color;
		}

		protected virtual void UpdateChannelG(object sender, EventArgs args)
		{
			var slider = sender as SliderBox;
			_color.G = (byte)Math.Round(slider.Current);
			sliderText[1].TextBoard.SetText($"G: {_color.G}");
			display.Color = _color;
		}

		protected virtual void UpdateChannelB(object sender, EventArgs args)
		{
			var slider = sender as SliderBox;
			_color.B = (byte)Math.Round(slider.Current);
			sliderText[2].TextBoard.SetText($"B: {_color.B}");
			display.Color = _color;
		}

		protected override void HandleInput(Vector2 cursorPos)
        {
            if (_color != lastColor)
            {
                ValueChanged?.Invoke(this, EventArgs.Empty);
                lastColor = _color;
            }

            if (focusedChannel != -1)
            {
                sliders[focusedChannel].FocusHandler.GetInputFocus();
                focusedChannel = -1;
            }

            for (int i = 0; i < sliders.Length; i++)
            {
                if (sliders[i].FocusHandler.HasFocus)
                {
                    if (SharedBinds.UpArrow.IsNewPressed)
                    {
                        i = MathHelper.Clamp(i - 1, 0, sliders.Length - 1);
                        sliders[i].FocusHandler.GetInputFocus();
                    }
                    else if (SharedBinds.DownArrow.IsNewPressed)
                    {
                        i = MathHelper.Clamp(i + 1, 0, sliders.Length - 1);
                        sliders[i].FocusHandler.GetInputFocus();
                    }

                    break;
                }
            }
        }
    }
}
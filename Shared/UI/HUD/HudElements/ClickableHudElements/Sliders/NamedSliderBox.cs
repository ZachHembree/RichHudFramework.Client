using System;
using System.Text;
using VRage;
using VRageMath;
using GlyphFormatMembers = VRage.MyTuple<byte, float, VRageMath.Vector2I, VRageMath.Color>;

namespace RichHudFramework.UI
{
    using UI.Rendering;

    /// <summary>
    /// Horizontal sliderbox with a name and value label. Value label is not updated automatically. Made to
    /// resemble sliders used in the SE terminal.
    /// </summary>
    public class NamedSliderBox : HudElementBase, IClickableElement
    {
		/// <summary>
		/// Invoked when the current value changes
		/// </summary>
		public event EventHandler ValueChanged
		{
			add { sliderBox.ValueChanged += value; }
			remove { sliderBox.ValueChanged -= value; }
		}

        /// <summary>
        /// Registers a value update callback. Useful in initializers.
        /// </summary>
        public EventHandler UpdateValueCallback
        {
            set { sliderBox.ValueChanged += value; }
        }

		/// <summary>
		/// The name of the control
		/// </summary>
		public RichText Name { get { return name.TextBoard.GetText(); } set { name.TextBoard.SetText(value); } }

        /// <summary>
        /// Text indicating the current value of the slider. Does not automatically reflect changes to the slider value.
        /// </summary>
        public RichText ValueText { get { return current.TextBoard.GetText(); } set { current.TextBoard.SetText(value); } }

        /// <summary>
        /// The name of the control
        /// </summary>
        public ITextBuilder NameBuilder => name.TextBoard;

        /// <summary>
        /// Text indicating the current value of the slider. Does not automatically reflect changes to the slider value.
        /// </summary>
        public ITextBuilder ValueBuilder => current.TextBoard;

        /// <summary>
        /// Minimum configurable value for the slider.
        /// </summary>
        public float Min { get { return sliderBox.Min; } set { sliderBox.Min = value; } }

        /// <summary>
        /// Maximum configurable value for the slider.
        /// </summary>
        public float Max { get { return sliderBox.Max; } set { sliderBox.Max = value; } }

        /// <summary>
        /// Value currently set on the slider.
        /// </summary>
        public float Current { get { return sliderBox.Current; } set { sliderBox.Current = value; } }

        /// <summary>
        /// Current slider value expreseed as a percentage between the min and maximum values.
        /// </summary>
        public float Percent { get { return sliderBox.Percent; } set { sliderBox.Percent = value; } }

		/// <summary>
		/// Interface used to manage the element's input focus state
		/// </summary>
		public IFocusHandler FocusHandler => sliderBox.FocusHandler;

		/// <summary>
		/// Mouse input interface for this clickable element
		/// </summary>
		public IMouseInput MouseInput => sliderBox.MouseInput;

		public override bool IsMousedOver => sliderBox.IsMousedOver;

        protected readonly Label name, current;
        protected readonly SliderBox sliderBox;

        public NamedSliderBox(HudParentBase parent) : base(parent)
        {
            sliderBox = new SliderBox(this)
            {
                DimAlignment = DimAlignments.UnpaddedWidth,
                ParentAlignment = ParentAlignments.InnerBottom,
                UseCursor = true,
            };

            name = new Label(this)
            {
                AutoResize = false,
                Format = TerminalFormatting.ControlFormat,
                Text = "NewSlideBox",
                Offset = new Vector2(0f, -18f),
                ParentAlignment = ParentAlignments.PaddedInnerLeft | ParentAlignments.Top
            };

            current = new Label(this)
            {
                AutoResize = false,
                Format = TerminalFormatting.ControlFormat.WithAlignment(TextAlignment.Right),
                Text = "Value",
                Offset = new Vector2(0f, -18f),
                ParentAlignment = ParentAlignments.PaddedInnerRight | ParentAlignments.Top
            };

            FocusHandler.InputOwner = this;
            Padding = new Vector2(40f, 0f);
            Size = new Vector2(317f, 70f);
        }

        public NamedSliderBox() : this(null)
        { }

        protected override void Layout()
        {
            Vector2 size = UnpaddedSize;
            current.UnpaddedSize = current.TextBoard.TextSize;
            name.UnpaddedSize = name.TextBoard.TextSize;
			sliderBox.Height = size.Y - Math.Max(name.Height, current.Height);
			current.Width = Math.Max(size.X - name.Width - 10f, 0f);
        }
    }
}
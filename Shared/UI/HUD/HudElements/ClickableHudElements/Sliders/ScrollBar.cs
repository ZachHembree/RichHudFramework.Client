using VRageMath;
using System;

namespace RichHudFramework.UI
{
    /// <summary>
    /// Clickable scrollbar. Designed to mimic the appearance of the scrollbars used in SE.
    /// </summary>
    public class ScrollBar : HudElementBase, IClickableElement
    {
		/// <summary>
		/// Invoked when the current value changes
		/// </summary>
		public event EventHandler ValueChanged
		{
			add { slide.ValueChanged += value; }
			remove { slide.ValueChanged -= value; }
		}

		/// <summary>
		/// Registers a value update callback. Useful in initializers.
		/// </summary>
		public EventHandler UpdateValueCallback
		{
			set { slide.ValueChanged += value; }
		}

		/// <summary>
		/// Minimum allowable value.
		/// </summary>
		public float Min
        {
            get { return slide.Min; }
            set { slide.Min = value; }
        }

        /// <summary>
        /// Maximum allowable value.
        /// </summary>
        public float Max
        {
            get { return slide.Max; }
            set { slide.Max = value; }
        }

        /// <summary>
        /// Currently set value. Clamped between min and max.
        /// </summary>
        public float Current { get { return slide.Current; } set { slide.Current = value; } }

        /// <summary>
        /// Current value expressed as a percentage over the range between the min and max values.
        /// </summary>
        public float Percent { get { return slide.Percent; } set { slide.Percent = value; } }

        /// <summary>
        /// Determines whether or not the scrollbar will be oriented vertically.
        /// </summary>
        public bool Vertical { get { return slide.Vertical; } set { slide.Vertical = value; slide.Reverse = value; } }

        /// <summary>
        /// Indicates whether or not the hud element is currently moused over
        /// </summary>
        public override bool IsMousedOver => slide.IsMousedOver;

		/// <summary>
		/// Interface used to manage the element's input focus state
		/// </summary>
		public IFocusHandler FocusHandler { get; }

		/// <summary>
		/// Mouse input interface for this clickable element
		/// </summary>
		public IMouseInput MouseInput => slide.MouseInput;

        public readonly SliderBar slide;

        public ScrollBar(HudParentBase parent) : base(parent)
        {
            FocusHandler = new InputFocusHandler(this);
            slide = new SliderBar(this)
            {
                Reverse = true,
                Vertical = true,
                SliderWidth = 13f,
                BarWidth = 13f,

                SliderColor = new Color(78, 87, 101),
                SliderHighlight = new Color(136, 140, 148),

                BarColor = new Color(41, 51, 61),
            };

            Size = new Vector2(13f, 300f);
            Padding = new Vector2(30f, 10f);
            slide.SliderVisible = false;
        }

        public ScrollBar() : this(null)
        { }

        protected override void Layout()
        {
            Vector2 size = UnpaddedSize;
            slide.BarSize = size;

            if (Vertical)
            {
                slide.SliderWidth = size.X;
                slide.SliderVisible = slide.SliderHeight < slide.BarHeight;
            }
            else
            {
                slide.SliderHeight = size.Y;
                slide.SliderVisible = slide.SliderWidth < slide.BarWidth;
            }
        }
    }    
}
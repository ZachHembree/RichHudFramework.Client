using VRageMath;

namespace RichHudFramework.UI
{
	/// <summary>
	/// A clickable scrollbar designed to mimic the appearance of standard Space Engineers scrollbars.
	/// Internally uses a <see cref="SliderBar"/>.
	/// </summary>
	public class ScrollBar : HudElementBase, IClickableElement
	{
		/// <summary>
		/// Invoked when the scrollbar value changes.
		/// </summary>
		public event EventHandler ValueChanged
		{
			add { slide.ValueChanged += value; }
			remove { slide.ValueChanged -= value; }
		}

		/// <summary>
		/// Helper property for registering a value update callback during initialization.
		/// </summary>
		public EventHandler UpdateValueCallback
		{
			set { slide.ValueChanged += value; }
		}

		/// <summary>
		/// The minimum allowable value.
		/// </summary>
		public float Min
		{
			get { return slide.Min; }
			set { slide.Min = value; }
		}

		/// <summary>
		/// The maximum allowable value.
		/// </summary>
		public float Max
		{
			get { return slide.Max; }
			set { slide.Max = value; }
		}

		/// <summary>
		/// The currently set value, clamped between <see cref="Min"/> and <see cref="Max"/>.
		/// </summary>
		public float Current { get { return slide.Current; } set { slide.Current = value; } }

		/// <summary>
		/// The current value expressed as a percentage (0 to 1) of the range between Min and Max.
		/// </summary>
		public float Percent { get { return slide.Percent; } set { slide.Percent = value; } }

		/// <summary>
		/// Determines whether the scrollbar is oriented vertically. If true, the slider operates on the Y-axis.
		/// </summary>
		public bool Vertical { get { return slide.Vertical; } set { slide.Vertical = value; slide.Reverse = value; } }

		/// <summary>
		/// Indicates whether the cursor is currently over the scrollbar.
		/// </summary>
		public override bool IsMousedOver => slide.IsMousedOver;

		/// <summary>
		/// Interface used to manage the element's input focus state.
		/// </summary>
		public IFocusHandler FocusHandler { get; }

		/// <summary>
		/// Mouse input interface for this clickable element.
		/// </summary>
		public IMouseInput MouseInput => slide.MouseInput;

		/// <summary>
		/// The internal slider element functioning as the scrollbar.
		/// </summary>
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

		/// <summary>
		/// Updates the scrollbar's slider size and visibility based on whether the content fits within the visible area.
		/// </summary>
		/// <exclude/>
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
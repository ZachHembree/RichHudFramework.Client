using System;
using VRageMath;

namespace RichHudFramework.UI
{
	using UI.Rendering;

	/// <summary>
	/// A composite control containing a horizontal slider box, a name label, and a value label. 
	/// <para>
	/// Note: The value label is not updated automatically; it must be updated manually via the ValueChanged event.
	/// </para>
	/// </summary>
	public class NamedSliderBox : HudElementBase, IClickableElement
	{
		/// <summary>
		/// Invoked when the current value of the slider changes.
		/// </summary>
		public event EventHandler ValueChanged
		{
			add { sliderBox.ValueChanged += value; }
			remove { sliderBox.ValueChanged -= value; }
		}

		/// <summary>
		/// Helper property for registering a value update callback during initialization.
		/// </summary>
		public EventHandler UpdateValueCallback
		{
			set { sliderBox.ValueChanged += value; }
		}

		/// <summary>
		/// The text displayed in the name label.
		/// </summary>
		public RichText Name { get { return name.TextBoard.GetText(); } set { name.TextBoard.SetText(value); } }

		/// <summary>
		/// The text displayed in the value label. 
		/// <para>Note: This does not automatically reflect changes to the slider value; you must update this string manually.</para>
		/// </summary>
		public RichText ValueText { get { return current.TextBoard.GetText(); } set { current.TextBoard.SetText(value); } }

		/// <summary>
		/// Accessor for the text builder of the name label.
		/// </summary>
		public ITextBuilder NameBuilder => name.TextBoard;

		/// <summary>
		/// Accessor for the text builder of the value label.
		/// </summary>
		public ITextBuilder ValueBuilder => current.TextBoard;

		/// <summary>
		/// The minimum configurable value for the slider.
		/// </summary>
		public float Min { get { return sliderBox.Min; } set { sliderBox.Min = value; } }

		/// <summary>
		/// The maximum configurable value for the slider.
		/// </summary>
		public float Max { get { return sliderBox.Max; } set { sliderBox.Max = value; } }

		/// <summary>
		/// The value currently set on the slider.
		/// </summary>
		public float Current { get { return sliderBox.Current; } set { sliderBox.Current = value; } }

		/// <summary>
		/// The current slider value expressed as a percentage (0 to 1) of the range between the Min and Max values.
		/// </summary>
		public float Percent { get { return sliderBox.Percent; } set { sliderBox.Percent = value; } }

		/// <summary>
		/// Interface used to manage the element's input focus state.
		/// </summary>
		public IFocusHandler FocusHandler => sliderBox.FocusHandler;

		/// <summary>
		/// Mouse input interface for this clickable element.
		/// </summary>
		public IMouseInput MouseInput => sliderBox.MouseInput;

		/// <summary>
		/// Indicates whether the cursor is currently over the slider box.
		/// </summary>
		public override bool IsMousedOver => sliderBox.IsMousedOver;

		/// <summary>
		/// Labels for the name and current value display.
		/// </summary>
		/// <exclude/>
		protected readonly Label name, current;

		/// <exclude/>
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

		/// <summary>
		/// Sizes the labels and slider box to fit within the element's bounds.
		/// </summary>
		/// <exclude/>
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
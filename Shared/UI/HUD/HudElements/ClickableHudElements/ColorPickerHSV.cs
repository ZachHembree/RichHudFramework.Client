using System;
using System.Text;
using RichHudFramework.UI.Rendering;
using VRageMath;

namespace RichHudFramework.UI
{
    using UI;

    /// <summary>
    /// Named color picker using sliders designed to mimic the appearance of the color picker in the SE terminal.
    /// HSV only. Alpha not supported.
    /// </summary>
    public class ColorPickerHSV : ColorPickerRGB
    {
		protected static readonly Vector3 HSVScale = 1f / new Vector3(360f, 100f, 100f);

		/// <summary>
		/// Currently selected color in HSV format
		/// </summary>
		public Vector3 ColorHSV => _hsvColor;

		protected Vector3 _hsvColor;

		public ColorPickerHSV(HudParentBase parent = null) : base(parent)
		{
			sliders[0].Max = 360f;
			sliders[1].Max = 100f;
			sliders[2].Max = 100f;
		}

		protected override void UpdateChannelR(object sender, EventArgs args)
		{
			var slider = sender as SliderBox;
			_hsvColor.X = (float)Math.Round(slider.Current);
			sliderText[0].TextBoard.SetText($"H: {_hsvColor.X}");

			_color = (_hsvColor * HSVScale).HSVtoColor();
			display.Color = _color;
		}

		protected override void UpdateChannelG(object sender, EventArgs args)
		{
			var slider = sender as SliderBox;
			_hsvColor.Y = (float)Math.Round(slider.Current);
			sliderText[1].TextBoard.SetText($"S: {_hsvColor.Y}");

			_color = (_hsvColor * HSVScale).HSVtoColor();
			display.Color = _color;
		}

		protected override void UpdateChannelB(object sender, EventArgs args)
		{
			var slider = sender as SliderBox;
			_hsvColor.Z = (float)Math.Round(slider.Current);
			sliderText[2].TextBoard.SetText($"V: {_hsvColor.Z}");

			_color = (_hsvColor * HSVScale).HSVtoColor();
			display.Color = _color;
		}
    }
}
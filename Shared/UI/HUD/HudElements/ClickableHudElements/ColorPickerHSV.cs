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
		protected override void UpdateChannelR(object sender, EventArgs args)
		{
			var slider = sender as SliderBox;
			_color.R = (byte)Math.Round(slider.Current);
			sliderText[0].TextBoard.SetText($"H: {_color.R}");
			display.Color = (_color / new Vector3(360f, 100f, 100f)).HSVtoColor();
		}

		protected override void UpdateChannelG(object sender, EventArgs args)
		{
			var slider = sender as SliderBox;
			_color.G = (byte)Math.Round(slider.Current);
			sliderText[1].TextBoard.SetText($"S: {_color.G}");
			display.Color = (_color / new Vector3(360f, 100f, 100f)).HSVtoColor();
		}

		protected override void UpdateChannelB(object sender, EventArgs args)
		{
			var slider = sender as SliderBox;
			_color.B = (byte)Math.Round(slider.Current);
			sliderText[2].TextBoard.SetText($"V: {_color.B}");
			display.Color = (_color / new Vector3(360f, 100f, 100f)).HSVtoColor();
		}
    }
}
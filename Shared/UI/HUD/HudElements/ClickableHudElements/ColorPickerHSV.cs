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
        protected override void Layout()
        {
            display.Color = (_color / new Vector3(360f, 100f, 100f)).HSVtoColor();
        }
    }
}
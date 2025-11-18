using System.Text;
using VRage;
using GlyphFormatMembers = VRage.MyTuple<byte, float, VRageMath.Vector2I, VRageMath.Color>;

namespace RichHudFramework.UI.Client
{
	/// <summary>
	/// On/Off toggle designed to mimic the appearance of the On/Off button in the SE Terminal.
	/// </summary>
	public class TerminalOnOffButton : TerminalValue<bool>
	{
		public TerminalOnOffButton() : base(MenuControls.OnOffButton)
		{ }
	}
}
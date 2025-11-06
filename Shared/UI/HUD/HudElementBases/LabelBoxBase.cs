using System;
using VRage;
using VRageMath;

namespace RichHudFramework
{
    namespace UI
    {
        using Client;
        using Server;

        /// <summary>
        /// Base type for elements combining Labels with textured backgrounds.
        /// </summary>
        public abstract class LabelBoxBase : HudElementBase
        {
            /// <summary>
            /// Size of the text element sans padding.
            /// </summary>
            public abstract Vector2 TextSize { get; set; }

            /// <summary>
            /// Padding applied to the text element.
            /// </summary>
            public abstract Vector2 TextPadding { get; set; }

            /// <summary>
            /// If true, the text will set its bounds equal to the total text size. If false, the text bounds
            /// are set manually. Setting sizes smaller than the TextSize will result in extraneous text being
            /// clipped or drawing outside the bounds of the element.
            /// </summary>
            public abstract bool AutoResize { get; set; }

            /// <summary>
            /// If true, then the background will resize to match the size of the text plus padding. Otherwise,
            /// size will be clamped such that the element will not be smaller than the text element. Does not 
            /// apply if AutoResize is disabled.
            /// </summary>
            public bool FitToTextElement { get; set; }

            /// <summary>
            /// Background color
            /// </summary>
            public virtual Color Color { get { return background.Color; } set { background.Color = value; } }

            /// <summary>
            /// Label box background
            /// </summary>
            public readonly TexturedBox background;

            public LabelBoxBase(HudParentBase parent) : base(parent)
            {
                background = new TexturedBox(this)
                {
                    DimAlignment = DimAlignments.UnpaddedSize,
                };

                FitToTextElement = true;
                Color = Color.Gray;
				UnpaddedSize = new Vector2(50f);
			}

            protected override void Measure()
            {
				if (AutoResize)
				{
                    if (FitToTextElement)
                        UnpaddedSize = TextSize;
                    else
                        UnpaddedSize = Vector2.Max(UnpaddedSize, TextSize);
				}
			}

			protected override void Layout()
			{
				if (!AutoResize)
				{
					TextSize = UnpaddedSize;
				}
			}
        }
    }
}
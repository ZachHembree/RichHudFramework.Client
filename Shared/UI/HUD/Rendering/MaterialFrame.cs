using VRageMath;

namespace RichHudFramework
{
    namespace UI
    {
        namespace Rendering
        {
            public interface IReadOnlyMaterialFrame
            {
                /// <summary>
                /// Texture associated with the frame
                /// </summary>
                MaterialAlignment Alignment { get; }

                /// <summary>
                /// Determines how or if the material is scaled w/respect to its aspect ratio.
                /// </summary>
                Material Material { get;}

                BoundingBox2 GetMaterialAlignment(float bbAspectRatio);
            }

            /// <summary>
            /// Defines the positioning and alignment of a Material on a QuadBoard.
            /// </summary>
            public class MaterialFrame : IReadOnlyMaterialFrame
            {
                /// <summary>
                /// Texture associated with the frame
                /// </summary>
                public Material Material { get; set; }

                /// <summary>
                /// Determines how or if the material is scaled w/respect to its aspect ratio.
                /// </summary>
                public MaterialAlignment Alignment { get; set; }

                public MaterialFrame()
                {
                    Material = Material.Default;
                    Alignment = MaterialAlignment.StretchToFit;
                }

                /// <summary>
                /// Calculates the texture coordinates needed to fit the material to the billboard. 
                /// Aspect ratio = Width/Height
                /// </summary>
                public BoundingBox2 GetMaterialAlignment(float bbAspectRatio)
                {
					BoundingBox2 bounds = Material.uvBounds;

					if (Alignment != MaterialAlignment.StretchToFit)
                    {
						Vector2 uvScale = new Vector2(1f);
						float matAspectRatio = Material.size.X / Material.size.Y;

                        if (Alignment == MaterialAlignment.FitAuto)
                        {
                            if (matAspectRatio > bbAspectRatio) // If material is too wide, make it shorter
                                uvScale = new Vector2(1f, matAspectRatio / bbAspectRatio);
                            else // If the material is too tall, make it narrower
                                uvScale = new Vector2(bbAspectRatio / matAspectRatio, 1f);
                        }
                        else if (Alignment == MaterialAlignment.FitVertical)
                        {
                            uvScale = new Vector2(bbAspectRatio / matAspectRatio, 1f);
                        }
                        else if (Alignment == MaterialAlignment.FitHorizontal)
                        {
                            uvScale = new Vector2(1f, matAspectRatio / bbAspectRatio);
                        }

						bounds.Scale(uvScale);
					}

					return bounds;
                }

                /// <summary>
                /// Returns scaling that needs to be applied to a billboard with the given aspect 
                /// ratio to remain consistent with the given Material Alignment without warping 
                /// texture coordinates.
                /// </summary>
                public Vector2 GetAlignmentScale(float bbAspectRatio) 
                {
                    if (Alignment != MaterialAlignment.StretchToFit)
                    {
                        float matAspectRatio = Material.size.X / Material.size.Y;
                        Vector2 bbScale = new Vector2(1f);

                        if (Alignment == MaterialAlignment.FitAuto)
                        {
                            if (matAspectRatio < bbAspectRatio)
                                bbScale = new Vector2(matAspectRatio / bbAspectRatio, 1f);
                            else
                                bbScale = new Vector2(1f, bbAspectRatio / matAspectRatio);
                        }
                        else if (Alignment == MaterialAlignment.FitHorizontal)
                        {
                            bbScale = new Vector2(1f, bbAspectRatio / matAspectRatio);
                        }
                        else if (Alignment == MaterialAlignment.FitVertical)
                        {
                            bbScale = new Vector2(matAspectRatio / bbAspectRatio, 1f);
                        }

                        return bbScale;
                    }
                    else
                        return Vector2.One;
				}
            }
        }
    }
}
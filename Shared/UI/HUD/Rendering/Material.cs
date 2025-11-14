using System.Net;
using VRage.Utils;
using VRageMath;

namespace RichHudFramework
{
    namespace UI
    {
        namespace Rendering
        {
            /// <summary>
            /// Used to determine how a given <see cref="Material"/> is scaled on a given Billboard.
            /// Note: texture colors are clamped to their edges.
            /// </summary>
            public enum MaterialAlignment : int
            {
                /// <summary>
                /// Stretches/compresses the material to cover the whole billboard. Default behavior.
                /// </summary>
                StretchToFit = 0,

                /// <summary>
                ///  Rescales the material so that it matches the height of the Billboard while maintaining its aspect ratio.
                ///  Material will be clipped as needed.
                /// </summary>
                FitVertical = 1,

                /// <summary>
                /// Rescales the material so that it matches the width of the Billboard while maintaining its aspect ratio.
                /// Material will be clipped as needed.
                /// </summary>
                FitHorizontal = 2,

                /// <summary>
                /// Rescales the material such that it maintains it's aspect ratio while filling as much of the billboard
                /// as possible
                /// </summary>
                FitAuto = 3,
            }

			/// <summary>
			/// Defines a texture used by <see cref="MatBoard"/>s. Supports sprite sheets. Acts as a handle
			/// to a Space Engineers Transparent Material, by SubtypeId, decorated with additional metadata for use 
            /// with the <see cref="VRage.Game.MyTransparentGeometry"/> API.
			/// </summary>
			public class Material
            {
                public static readonly Material 
                    // Blank solid texture - used for plain color UI elements
                    Default = new Material("RichHudDefault", new Vector2(4f, 4f)),
                    // A perfect circle 1024^2
                    CircleMat = new Material("RhfCircle", new Vector2(1024f)),
                    // Donut shape 1024^2
                    AnnulusMat = new Material("RhfAnnulus", new Vector2(1024f));

				/// <summary>
				/// SubtypeId of the Transparent Material the <see cref="Material"/> is based on.
				/// </summary>
				public readonly MyStringId TextureID;

                /// <summary>
                /// The dimensions, in pixels, of the <see cref="Material"/>.
                /// </summary>
                public readonly Vector2 Size;

                /// <summary>
                /// Minimum and maximum bounds in normalized texture coordinates
                /// </summary>
                public readonly BoundingBox2 UVBounds;

				/// <summary>
				/// Creates a <see cref="Material"/> using the SubtypeId of a Transparent Material 
                /// and the original dimensions, in pixels.
				/// </summary>
				/// <param name="SubtypeId">Name of the texture ID</param>
				/// <param name="size">Size of the material in pixels</param>
				public Material(string SubtypeId, Vector2 size) : this(MyStringId.GetOrCompute(SubtypeId), size)
                { }

				/// <summary>
				/// Creates a <see cref="Material"/> from a subsection of a texture atlas based on a 
				/// Transparent Material with a given SubtypeId.
				/// </summary>
				/// <param name="SubtypeId">Name of the texture ID</param>
				/// <param name="texSize">Size of the texture associated with the SubtypeId in pixels</param>
				/// <param name="texCoords">UV offset starting from the upper left hand corner in pixels</param>
				/// <param name="size">Size of the material starting from the given offset</param>
				public Material(string SubtypeId, Vector2 texSize, Vector2 texCoords, Vector2 size)
                    : this(MyStringId.GetOrCompute(SubtypeId), texSize, texCoords, size)
                { }

				/// <summary>
				/// Creates a <see cref="Material"/> using the SubtypeId of a Transparent Material 
				/// and the original dimensions, in pixels.
				/// </summary>
				/// <param name="TextureID">MyStringID associated with the texture SubtypeId</param>
				/// <param name="size">Size of the material in pixels</param>
				public Material(MyStringId TextureID, Vector2 size)
                {
                    this.TextureID = TextureID;
                    this.Size = size;
                    UVBounds = new BoundingBox2(Vector2.Zero, Vector2.One);
                }

				/// <summary>
				/// Creates a <see cref="Material"/> from a subsection of a texture atlas based on a 
				/// Transparent Material with a given SubtypeId.
				/// </summary>
				/// <param name="SubtypeId">MyStringID associated with the texture SubtypeId</param>
				/// <param name="texSize">Size of the texture associated with the SubtypeId in pixels</param>
				/// <param name="offset">Texture offset starting from the upper left hand corner in pixels</param>
				/// <param name="size">Size of the material starting from the given offset</param>
				public Material(MyStringId SubtypeId, Vector2 texSize, Vector2 offset, Vector2 size)
                {
                    this.TextureID = SubtypeId;
                    this.Size = size;

                    Vector2 rcpTexSize = 1f / texSize,
                        halfUVSize = .5f * size * rcpTexSize,
                        uvOffset = (offset * rcpTexSize) + halfUVSize;

                    UVBounds = new BoundingBox2(uvOffset - halfUVSize, uvOffset + halfUVSize);
                }
            }

        }
    }
}
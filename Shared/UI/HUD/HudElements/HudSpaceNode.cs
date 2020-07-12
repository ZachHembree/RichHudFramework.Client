using System;
using VRage;
using VRageMath;

namespace RichHudFramework
{
    namespace UI
    {
        /// <summary>
        /// HUD node used to replace the standard Pixel to World matrix with an arbitrary
        /// world matrix transform. Typically parented to HudMain.Root.
        /// </summary>
        public class HudSpaceNode : HudNodeBase
        {
            /// <summary>
            /// Returns the current draw matrix
            /// </summary>
            public MatrixD CustomMatrix => _customMatrix;

            /// <summary>
            /// Used to update the current draw matrix. If no delegate is set, the node will default
            /// to the matrix supplied by its parent.
            /// </summary>
            public Func<MatrixD> UpdateMatrixFunc;

            private MatrixD _customMatrix;

            public HudSpaceNode(IHudParent parent = null) : base(parent)
            { }

            public override void BeforeDraw(HudLayers layer, ref MatrixD oldMatrix)
            {
                if (UpdateMatrixFunc != null)
                    _customMatrix = UpdateMatrixFunc();
                else
                    _customMatrix = oldMatrix;

                base.BeforeDraw(layer, ref _customMatrix);
            }
        }
    }
}

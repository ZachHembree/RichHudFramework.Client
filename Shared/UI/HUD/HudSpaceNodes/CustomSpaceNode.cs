using Sandbox.ModAPI;
using System;
using VRage;
using VRage.ModAPI;
using VRageMath;
using ApiMemberAccessor = System.Func<object, int, object>;
using HudSpaceDelegate = System.Func<VRage.MyTuple<bool, float, VRageMath.MatrixD>>;

namespace RichHudFramework
{
    namespace UI
    {
        using Client;
        using Server;
        using System.Collections.Generic;
        using HudUpdateAccessors = MyTuple<
            ApiMemberAccessor,
            MyTuple<Func<ushort>, Func<Vector3D>>, // ZOffset + GetOrigin
            Action, // DepthTest
            Action, // HandleInput
            Action<bool>, // BeforeLayout
            Action // BeforeDraw
        >;

        /// <summary>
        /// HUD node used to replace the standard Pixel to World matrix with an arbitrary
        /// world matrix transform given by a user-supplied delegate.
        /// </summary>
        public class CustomSpaceNode : HudSpaceNodeBase
        {
            /// <summary>
            /// Used to update the current draw matrix. If no delegate is set, the node will default
            /// to the matrix supplied by its parent.
            /// </summary>
            public Func<MatrixD> UpdateMatrixFunc { get; set; }

            public CustomSpaceNode(HudParentBase parent = null) : base(parent)
            {
                GetHudSpaceFunc = () => new MyTuple<bool, float, MatrixD>(DrawCursorInHudSpace, Scale, PlaneToWorld);
                DrawCursorInHudSpace = true;
                GetNodeOriginFunc = () => PlaneToWorld.Translation;
            }

            protected override void BeginLayout(bool refresh)
            {
                if (UpdateMatrixFunc != null)
                    PlaneToWorld = UpdateMatrixFunc();
                else if (Parent?.HudSpace != null)
                    PlaneToWorld = Parent.HudSpace.PlaneToWorld;

                base.BeginLayout(refresh);
            }
        }
    }
}

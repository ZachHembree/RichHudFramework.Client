using System;
using VRage;
using VRageMath;
using ApiMemberAccessor = System.Func<object, int, object>;

namespace RichHudFramework
{
    using HudElementMembers = MyTuple<
        Func<bool>, // Visible
        object, // ID
        Action<bool>, // BeforeLayout
        Action<int, MatrixD>, // BeforeDraw
        Action<int>, // HandleInput
        ApiMemberAccessor // GetOrSetMembers
    >;

    namespace UI
    {
        /// <summary>
        /// Wrapper used to access types of <see cref="IHudNode"/> via the API.
        /// </summary>
        public sealed class HudNodeData : HudParentData, IHudNode, IReadOnlyHudNode
        {
            public IHudParent Parent
            {
                get
                {
                    var id = GetOrSetMemberFunc(null, (int)HudNodeAccessors.GetParentID);

                    if (parent?.ID != id)
                    {
                        if (id != null)
                        {
                            var localParent = id as HudParentBase;

                            if (localParent != null)
                                parent = localParent;
                            else
                                parent = new HudParentData((HudElementMembers)GetOrSetMemberFunc(null, (int)HudNodeAccessors.GetParentData));
                        }
                        else
                            parent = null;
                    }

                    return parent;
                }
            }

            public HudLayers ZOffset 
            {
                get { return (HudLayers)GetOrSetMemberFunc(null, (int)HudNodeAccessors.ZOffset); }
                set { GetOrSetMemberFunc(value, (int)HudNodeAccessors.ZOffset); }
            }

            public bool Registered => (bool)GetOrSetMemberFunc(null, (int)HudNodeAccessors.Registered);

            public float Scale 
            { 
                get { return (float)GetOrSetMemberFunc(null, (int)HudNodeAccessors.Scale); } 
                set { GetOrSetMemberFunc(value, (int)HudNodeAccessors.Scale); } 
            }

            private IHudParent parent;

            public HudNodeData(HudElementMembers apiData) : base(apiData)
            { }

            public void Register(IHudParent parent) =>
                GetOrSetMemberFunc(parent.GetApiData(), (int)HudNodeAccessors.Register);

            public void GetFocus() =>
                GetOrSetMemberFunc(null, (int)HudNodeAccessors.GetFocus);

            public void Unregister() =>
                GetOrSetMemberFunc(null, (int)HudNodeAccessors.Unregister);
        }
    }
}
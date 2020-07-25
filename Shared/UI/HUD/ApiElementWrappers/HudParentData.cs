using System;
using System.Collections.Generic;
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
        /// Wrapper used to access types of <see cref="IHudParent"/> via the API.
        /// </summary>
        public class HudParentData : IHudParent, IReadOnlyHudParent
        {
            public bool Visible { get { return VisFunc(); } set { } }

            public object ID { get; }

            private readonly List<object> localChildren;

            private readonly Func<bool> VisFunc;
            private readonly Action<bool> BeforeLayoutAction;
            private readonly Action<int, MatrixD> BeforeDrawAction;
            private readonly Action<int> BeforeInputAction;
            protected readonly ApiMemberAccessor GetOrSetMemberFunc;

            public HudParentData(HudElementMembers members)
            {
                localChildren = new List<object>();

                VisFunc = members.Item1;
                ID = members.Item2;
                BeforeLayoutAction = members.Item3;
                BeforeDrawAction = members.Item4;
                BeforeInputAction = members.Item5;
                GetOrSetMemberFunc = members.Item6;
            }

            public void RegisterChild(IHudNode child)
            {
                GetOrSetMemberFunc(child.GetApiData(), (int)HudParentAccessors.Add);

                if (child.Parent.ID == ID)
                    localChildren.Add(child.ID);
            }

            public void BeforeLayout(bool refresh) =>
                BeforeLayoutAction(refresh);

            public void BeforeDraw(HudLayers layer, ref MatrixD matrix) =>
                BeforeDrawAction((int)layer, matrix);

            public void BeforeInput(HudLayers layer) =>
                BeforeInputAction((int)layer);

            public void RegisterChildren(IList<IHudNode> newChildren)
            {
                foreach (IHudNode child in newChildren)
                    child.Register(this);
            }

            public void RemoveChild(IHudNode child)
            {
                localChildren.Remove(child.ID);
                GetOrSetMemberFunc(child.ID, (int)HudParentAccessors.RemoveChild);
            }

            public void SetFocus(IHudNode child) =>
                GetOrSetMemberFunc(child.ID, (int)HudParentAccessors.SetFocus);

            public void ClearLocalChildren()
            {
                for (int n = 0; n < localChildren.Count; n++)
                    GetOrSetMemberFunc(localChildren[n], (int)HudParentAccessors.RemoveChild);

                localChildren.Clear();
            }

            public HudElementMembers GetApiData()
            {
                return new HudElementMembers()
                {
                    Item1 = VisFunc,
                    Item2 = ID,
                    Item3 = BeforeLayoutAction,
                    Item4 = BeforeDrawAction,
                    Item5 = BeforeInputAction,
                    Item6 = GetOrSetMemberFunc
                };
            }
        }
    }
}
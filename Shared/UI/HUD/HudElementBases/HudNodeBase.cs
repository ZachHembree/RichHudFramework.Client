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
        /// API accessors for IHudNode
        /// </summary>
        public enum HudNodeAccessors : int
        {
            GetParentID = 10,
            GetParentData = 11,
            GetFocus = 12,
            Register = 13,
            Unregister = 14,
            Registered = 15,
            Scale = 16
        }

        /// <summary>
        /// Base class for hud elements that can be parented to other elements.
        /// </summary>
        public abstract class HudNodeBase : HudParentBase, IHudNode, IReadOnlyHudNode
        {
            /// <summary>
            /// Parent object of the node.
            /// </summary>
            public virtual IHudParent Parent
            {
                get { return _parent; }
                protected set
                {
                    _parent = value;
                    _parentNode = value as HudNodeBase;
                }
            }

            /// <summary>
            /// Determines whether the UI element will be drawn in the Back, Mid or Foreground
            /// </summary>
            public HudLayers ZOffset
            {
                get { return _zOffset; }
                set { _zOffset = value; }
            }

            /// <summary>
            /// Scales the size and offset of an element. Any offset or size set at a given
            /// be increased or decreased with scale. Defaults to 1f. Includes parent scale.
            /// </summary>
            public virtual float Scale
            {
                get { return _scale; }
                set
                {
                    localScale = value;
                    _scale = _parentNode == null ? value : (value * _parentNode._scale);
                }
            }

            /// <summary>
            /// Indicates whether or not the element has been registered to a parent.
            /// </summary>
            public bool Registered { get; private set; }

            private IHudParent _parent;
            protected HudNodeBase _parentNode;
            protected float _scale, localScale;

            public HudNodeBase(IHudParent parent)
            {
                localScale = 1f;
                _scale = 1f;
                Register(parent);
            }

            public override void BeforeLayout(bool refresh)
            {
                _scale = _parentNode == null ? localScale : (localScale * _parentNode._scale);
                base.BeforeLayout(refresh);
            }

            /// <summary>
            /// Moves the element to the end of its parent's update list in order to ensure
            /// that it's drawn/updated last.
            /// </summary>
            public void GetFocus() =>
                Parent?.SetFocus(this);

            /// <summary>
            /// Registers the element to the given parent object.
            /// </summary>
            public virtual void Register(IHudParent parent)
            {
                if (parent != null && parent.ID == ID)
                    throw new Exception("Types of HudNodeBase cannot be parented to themselves!");

                if (parent != null && Parent == null)
                {
                    Parent = parent;
                    Parent.RegisterChild(this);
                    Registered = true;
                }

                if (_parentNode != null)
                {
                    _zOffset = _parentNode.ZOffset;
                    _scale = localScale * _parentNode._scale;
                }
                else
                    _scale = localScale;
            }

            /// <summary>
            /// Unregisters the element from its parent, if it has one.
            /// </summary>
            public virtual void Unregister()
            {
                if (Parent != null)
                {
                    IHudParent lastParent = Parent;

                    Parent = null;
                    lastParent.RemoveChild(this);
                    Registered = false;
                }

                _scale = localScale;
            }

            protected override object GetOrSetMember(object data, int memberEnum)
            {
                if (memberEnum < 10)
                {
                    base.GetOrSetMember(data, memberEnum);
                }
                else
                {
                    switch ((HudNodeAccessors)memberEnum)
                    {
                        case HudNodeAccessors.GetFocus:
                            GetFocus();
                            break;
                        case HudNodeAccessors.GetParentData:
                            return Parent.GetApiData();
                        case HudNodeAccessors.GetParentID:
                            return Parent?.ID;
                        case HudNodeAccessors.Register:
                            Register(new HudNodeData((HudElementMembers)data));
                            break;
                        case HudNodeAccessors.Unregister:
                            Unregister();
                            break;
                        case HudNodeAccessors.Registered:
                            return Registered;
                        case HudNodeAccessors.Scale:
                            if (data == null)
                                return Scale;
                            else
                            {
                                Scale = (float)data;
                                break;
                            }
                    }
                }

                return null;
            }
        }
    }
}
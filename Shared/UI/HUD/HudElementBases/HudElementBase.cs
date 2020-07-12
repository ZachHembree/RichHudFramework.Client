using System;
using VRage;
using VRageMath;

namespace RichHudFramework
{
    namespace UI
    {
        using Server;
        using Client;

        /// <summary>
        /// Base type for all hud elements with definite size and position. Inherits from HudParentBase and HudNodeBase.
        /// </summary>
        public abstract class HudElementBase : HudNodeBase, IHudElement, IReadOnlyHudElement
        {
            /// <summary>
            /// Parent object of the node.
            /// </summary>
            public sealed override IHudParent Parent
            {
                get { return base.Parent; }
                protected set
                {
                    base.Parent = value;
                    _parentFull = value as HudElementBase;
                }
            }

            /// <summary>
            /// Size of the element. Units in pixels by default.
            /// </summary>
            public Vector2 Size
            {
                get { return new Vector2(Width, Height); }
                set { Width = value.X; Height = value.Y; }
            }

            /// <summary>
            /// Width of the hud element. Units in pixels by default.
            /// </summary>
            public virtual float Width
            {
                get { return (_width * _scale) + Padding.X; }
                set
                {
                    if (Padding.X < value)
                        _width = (value - Padding.X) / _scale;
                    else
                        _width = (value / _scale);
                }
            }

            /// <summary>
            /// Height of the hud element. Units in pixels by default.
            /// </summary>
            public virtual float Height
            {
                get { return (_height * _scale) + Padding.Y; }
                set
                {
                    if (Padding.Y < value)
                        _height = (value - Padding.Y) / _scale;
                    else
                        _height = (value / _scale);
                }
            }

            /// <summary>
            /// Border size. Included in total element size.
            /// </summary>
            public virtual Vector2 Padding { get { return _padding * _scale; } set { _padding = value / _scale; } }

            /// <summary>
            /// Starting position of the hud element.
            /// </summary>
            public Vector2 Origin => (_parentFull == null) ? Vector2.Zero : (_parentFull.Origin + _parentFull.Offset + originAlignment);

            /// <summary>
            /// Position of the element relative to its origin.
            /// </summary>
            public virtual Vector2 Offset { get { return _offset * _scale; } set { _offset = value / _scale; } }

            /// <summary>
            /// Absolute position of the hud element. Origin + Offset.
            /// </summary>
            public Vector2 Position => Origin + Offset;

            /// <summary>
            /// Determines the starting position of the hud element relative to its parent.
            /// </summary>
            public ParentAlignments ParentAlignment { get; set; }

            /// <summary>
            /// Determines how/if an element will copy its parent's dimensions. 
            /// </summary>
            public DimAlignments DimAlignment { get; set; }

            /// <summary>
            /// If set to true the hud element will be allowed to capture the cursor.
            /// </summary>
            public bool CaptureCursor { get; set; }

            /// <summary>
            /// If set to true the hud element will share the cursor with other elements.
            /// </summary>
            public bool ShareCursor { get; set; }

            /// <summary>
            /// Indicates whether or not the element is capturing the cursor.
            /// </summary>
            public virtual bool IsMousedOver => Visible && isMousedOver;

            private const float minMouseBounds = 8f;
            private bool isMousedOver;
            private Vector2 _offset, _padding, originAlignment;

            protected float _width, _height;
            protected HudElementBase _parentFull;

            protected Vector2 cachedOrigin, cachedPosition, cachedSize;

            /// <summary>
            /// Initializes a new hud element with cursor sharing enabled and scaling set to 1f.
            /// </summary>
            public HudElementBase(IHudParent parent) : base(parent)
            {
                DimAlignment = DimAlignments.None;
                ParentAlignment = ParentAlignments.Center;
                ShareCursor = true;
            }

            public sealed override void BeforeInput(HudLayers layer)
            {
                if (Visible)
                {
                    for (int n = children.Count - 1; n >= 0; n--)
                    {
                        if (children[n].Visible)
                            children[n].BeforeInput(layer);
                    }

                    if (_zOffset == layer)
                    {
                        if (CaptureCursor && HudMain.Cursor.Visible && !HudMain.Cursor.IsCaptured)
                        {
                            isMousedOver = IsMouseInBounds();
                            HandleInput();

                            if (!ShareCursor && isMousedOver)
                                HudMain.Cursor.Capture(ID);
                        }
                        else
                        {
                            isMousedOver = false;
                            HandleInput();
                        }
                    }
                }
                else
                    isMousedOver = false;
            }

            /// <summary>
            /// Determines whether or not the cursor is within the bounds of the hud element.
            /// </summary>
            private bool IsMouseInBounds()
            {
                Vector2 cursorPos = HudMain.Cursor.Origin;
                float
                    width = Math.Max(minMouseBounds, cachedSize.X),
                    height = Math.Max(minMouseBounds, cachedSize.Y),
                    leftBound = cachedPosition.X - width / 2f,
                    rightBound = cachedPosition.X + width / 2f,
                    upperBound = cachedPosition.Y + height / 2f,
                    lowerBound = cachedPosition.Y - height / 2f;

                return
                    (cursorPos.X >= leftBound && cursorPos.X < rightBound) &&
                    (cursorPos.Y >= lowerBound && cursorPos.Y < upperBound);
            }

            public sealed override void BeforeLayout(bool refresh)
            {
                UpdateCache();
                Layout();

                for (int n = 0; n < children.Count; n++)
                {
                    if (children[n].Visible || refresh)
                        children[n].BeforeLayout(refresh);
                }
            }

            private void UpdateCache()
            {
                _scale = _parentNode == null ? localScale : (localScale * _parentNode.Scale);
                cachedSize = new Vector2(Width, Height);

                if (_parentFull != null)
                {
                    GetDimAlignment();
                    originAlignment = GetParentAlignment();
                    cachedOrigin = _parentFull.cachedPosition + originAlignment;
                }
                else
                    cachedOrigin = Vector2.Zero;

                cachedPosition = cachedOrigin + Offset;
            }

            /// <summary>
            /// Updates element dimensions to match those of its parent in accordance
            /// with its DimAlignment.
            /// </summary>
            private void GetDimAlignment()
            {
                if (DimAlignment != DimAlignments.None)
                {
                    float width = cachedSize.X, height = cachedSize.Y,
                        parentWidth = _parentFull.cachedSize.X, parentHeight = _parentFull.cachedSize.Y;

                    if ((DimAlignment & DimAlignments.IgnorePadding) == DimAlignments.IgnorePadding)
                    {
                        Vector2 parentPadding = _parentFull.Padding;

                        if ((DimAlignment & DimAlignments.Width) == DimAlignments.Width)
                            width = parentWidth - parentPadding.X;

                        if ((DimAlignment & DimAlignments.Height) == DimAlignments.Height)
                            height = parentHeight - parentPadding.Y;
                    }
                    else
                    {
                        if ((DimAlignment & DimAlignments.Width) == DimAlignments.Width)
                            width = parentWidth;

                        if ((DimAlignment & DimAlignments.Height) == DimAlignments.Height)
                            height = parentHeight;
                    }

                    Width = width;
                    Height = height;
                }
            }

            /// <summary>
            /// Calculates the offset necessary to achieve the alignment specified by the
            /// ParentAlignment property.
            /// </summary>
            private Vector2 GetParentAlignment()
            {
                Vector2 alignment = Vector2.Zero,
                    max = (_parentFull.cachedSize + cachedSize) / 2f,
                    min = -max;

                if ((ParentAlignment & ParentAlignments.UsePadding) == ParentAlignments.UsePadding)
                {
                    Vector2 parentPadding = _parentFull.Padding;

                    min += parentPadding / 2f;
                    max -= parentPadding / 2f;
                }

                if ((ParentAlignment & ParentAlignments.InnerV) == ParentAlignments.InnerV)
                {
                    min.Y += cachedSize.Y;
                    max.Y -= cachedSize.Y;
                }

                if ((ParentAlignment & ParentAlignments.InnerH) == ParentAlignments.InnerH)
                {
                    min.X += cachedSize.X;
                    max.X -= cachedSize.X;
                }

                if ((ParentAlignment & ParentAlignments.Bottom) == ParentAlignments.Bottom)
                    alignment.Y = min.Y;
                else if ((ParentAlignment & ParentAlignments.Top) == ParentAlignments.Top)
                    alignment.Y = max.Y;

                if ((ParentAlignment & ParentAlignments.Left) == ParentAlignments.Left)
                    alignment.X = min.X;
                else if ((ParentAlignment & ParentAlignments.Right) == ParentAlignments.Right)
                    alignment.X = max.X;

                return alignment;
            }
        }
    }
}
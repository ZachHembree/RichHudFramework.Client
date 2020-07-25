namespace RichHudFramework
{
    namespace UI
    {
        /// <summary>
        /// Used to determine which layer a UI element will be drawn on.
        /// Back/Mid/Foreground
        /// </summary>
        public enum HudLayers : int
        {
            None = -1,
            Background = 0,
            Midground = 1,
            Foreground = 2,
        }

        /// <summary>
        /// Read-only interface for hud elements that can be parented to another element.
        /// </summary>
        public interface IReadOnlyHudNode : IReadOnlyHudParent
        {
            /// <summary>
            /// Parent object of the node.
            /// </summary>
            IHudParent Parent { get; }

            /// <summary>
            /// Determines 
            /// </summary>
            HudLayers ZOffset { get; }

            /// <summary>
            /// Scales the size and offset of an element. Any offset or size set at a given
            /// be increased or decreased with scale. Defaults to 1f. Includes parent scale.
            /// </summary>
            float Scale { get; }

            /// <summary>
            /// Indicates whether or not the node has been registered to its parent.
            /// </summary>
            bool Registered { get; }
        }

        /// <summary>
        /// Interface for hud elements that can be parented to another element.
        /// </summary>
        public interface IHudNode : IHudParent
        {
            /// <summary>
            /// Parent object of the node.
            /// </summary>
            IHudParent Parent { get; }

            /// <summary>
            /// Determines whether the UI element will be drawn in the Back, Mid or Foreground
            /// </summary>
            HudLayers ZOffset { get; set; }

            /// <summary>
            /// Scales the size and offset of an element. Any offset or size set at a given
            /// be increased or decreased with scale. Defaults to 1f. Includes parent scale.
            /// </summary>
            float Scale { get; set; }

            /// <summary>
            /// Indicates whether or not the node has been registered to its parent.
            /// </summary>
            bool Registered { get; }

            /// <summary>
            /// Registers the element to the given parent object.
            /// </summary>
            void Register(IHudParent parent);

            /// <summary>
            /// Unregisters the element from its parent, if it has one.
            /// </summary>
            void Unregister();

            /// <summary>
            /// Moves the element to the end of its parent's update list in order to ensure
            /// that it's drawn/updated last.
            /// </summary>
            void GetFocus();
        }
    }
}
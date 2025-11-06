using System;

namespace RichHudFramework.UI
{
    /// <summary>
    /// Flags used to block input for the game's controls. Useful for modifying normal
    /// input behavior. Does not affect all binds.
    /// </summary>
    [Flags]
    public enum SeBlacklistModes : int
    {
        /// <summary>
        /// Default: no blacklist.
        /// </summary>
        None = 0,

        /// <summary>
        /// Set flag to disable mouse button keybinds. 
        /// </summary>
        Mouse = 1 << 0,

        /// <summary>
        /// Set flag to blacklist every blacklist-able bind. 
        /// Keep in mind that not every SE bind can be disabled.
        /// </summary>
        AllKeys = 1 << 1 | Mouse,

        /// <summary>
        /// Set flag to disable camera rotation (does not disable look with alt)
        /// </summary>
        CameraRot = 1 << 2,

        /// <summary>
        /// Set flag to disable mouse buttons as well as camera rotation.
        /// </summary>
        MouseAndCam = Mouse | CameraRot,

        /// <summary>
        /// Set flag to disable all key binds as well as camera rotation
        /// </summary>
        Full = AllKeys | CameraRot,

        /// <summary>
        /// Set flag to intercept chat messages
        /// </summary>
        Chat = 1 << 3,

        /// <summary>
        /// Set to blacklist every possible bind and intercept chat messages
        /// </summary>
        FullWithChat = Full | Chat
    }

    public enum BindClientAccessors : int
    {
        /// <summary>
        /// in: string, out: int
        /// </summary>
        GetOrCreateGroup = 1,

        /// <summary>
        /// in: string, out: int
        /// </summary>
        GetBindGroup = 2,

        /// <summary>
        /// in: IReadOnlyList{string}, out: int[]
        /// </summary>
        GetComboIndices = 3,

        /// <summary>
        /// in: string, out: int
        /// </summary>
        GetControlByName = 4,

        /// <summary>
        /// void
        /// </summary>
        ClearBindGroups = 5,

        /// <summary>
        /// void
        /// </summary>
        Unload = 6,

        /// <summary>
        /// in/out: SeBlacklistModes
        /// </summary>
        RequestBlacklistMode = 7,

        /// <summary>
        /// out: bool
        /// </summary>
        IsChatOpen = 8,

        /// <summary>
        /// in: int, out: string
        /// </summary>
        GetControlName = 9,

        /// <summary>
        /// in: IReadOnlyList{int}, out: string[]
        /// </summary>
        GetControlNames = 10,
    }
}
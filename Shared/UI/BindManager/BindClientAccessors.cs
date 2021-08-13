using System;

namespace RichHudFramework.UI
{
    /// <summary>
    /// Flags used to request a given blacklist configuration for SE controls.
    /// </summary>
    [Flags]
    public enum SeBlacklistModes : int
    {
        None = 0x0,

        /// <summary>
        /// Enable to disable mouse button keybinds. 
        /// </summary>
        Mouse = 0x1,

        /// <summary>
        /// Enable to blacklist every blacklist-able bind. 
        /// Keep in mind that not every SE bind can be disabled.
        /// </summary>
        Full = 0x2 | Mouse
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
    }
}
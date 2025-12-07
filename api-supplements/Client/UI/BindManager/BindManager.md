---
uid: RichHudFramework.UI.Client.BindManager
remarks: *content
---

The `BindManager` acts as the central hub for the creation, retrieval, and management of key bindings within the Rich HUD Framework. It facilitates the organization of controls into groups, manages the registration of custom input configurations, and handles the suppression of standard game inputs (blacklisting) when necessary.

### Key Components and Utilities

The input system is built around several core interfaces and classes designed to modularize input handling:

- <xref:RichHudFramework.UI.IBindGroup>: A collection of unique, order-independent key combinations as <xref:RichHudFramework.UI.IBind>s. This is the primary interface for organizing, initializing, and modifying binds.
    - Groups are created via <xref:RichHudFramework.UI.Client.BindManager.GetOrCreateGroup*>. Their names are local to a given mod, and must be unique (case-insensitive) within that context.
    - Bind names must be unique within a group. While the original casing of a name is preserved for display, the uniqueness constraint is case-insensitive.
    - Binds can be added individually via <xref:RichHudFramework.UI.IBindGroup.AddBind*> or <xref:RichHudFramework.UI.IBindGroup.TryRegisterBind*>, or in batches via <xref:RichHudFramework.UI.IBindGroup.RegisterBinds*>, but they cannot be removed once registered.
    - Binds are backed by <xref:RichHudFramework.UI.IControl> objects, which provide metadata and allow for dynamic configuration.
- <xref:RichHudFramework.UI.BindGroupInitializer>: A convenience container used to define the initial state of an `IBindGroup`.
    - Used in conjunction with `RegisterBinds` to batch-register default configurations.
    - Supports combinations of control names, `VRage.Input.MyKeys`, `VRage.Input.MyJoystickButtonsEnum`, and <xref:RichHudFramework.UI.RichHudControls>.
    - `RichHudControls` is the recommended enum for specifying hard-coded controls, as it unifies keyboard, mouse, and gamepad inputs into a single type.
- <xref:RichHudFramework.UI.Client.RebindPage>: A ready-made GUI for the <xref:RichHudFramework.UI.Client.RichHudTerminal> designed to manage key bindings. It provides an interactive user interface for clearing or rebinding keys within `IBindGroup`s registered to that page.
- <xref:RichHudFramework.UI.BindInputElement>: An implementation of <xref:RichHudFramework.UI.IBindInput> designed to integrate event-driven input into the UI tree.
    - While `IBind` provides global input events, `BindInputElement` ensures events only fire when the specific UI node is visible and input-enabled.
    - This is essential for context-sensitive controls that should not trigger when the UI is hidden or when a different window has focus.

> Key binds within a `IBindGroup` _must_ be unique. Different groups can have overlapping binds, but conflicts within groups are not allowed.

### Serialization
Key binds are **not** serialized automatically by the framework. It is the responsibility of the mod developer to handle persistence using the following workflow:

1.  **Save:** Before the session unloads, retrieve the current bind definitions using <xref:RichHudFramework.UI.IBindGroup.GetBindDefinitions*>.
2.  **Write:** Serialize these definitions (e.g., to XML) and write them to a configuration file in mod-local storage using [WriteFileInLocalStorage](https://keensoftwarehouse.github.io/SpaceEngineersModAPI/api/VRage.Game.ModAPI.IMyUtilities.html#VRage_Game_ModAPI_IMyUtilities_WriteFileInLocalStorage_System_String_System_String_System_Type_).
3.  **Read:** On session initialization, read the file using [ReadFileInLocalStorage](https://keensoftwarehouse.github.io/SpaceEngineersModAPI/api/VRage.Game.ModAPI.IMyUtilities.html#VRage_Game_ModAPI_IMyUtilities_ReadFileInLocalStorage_System_String_System_Type_) and deserialize the data.
4.  **Load:** Apply the loaded definitions to the `IBindGroup` using <xref:RichHudFramework.UI.IBindGroup.TryLoadBindData*>.

> Bind names should be registered via `RegisterBinds` *before* attempting to load deserialized definitions to ensure the group structure exists.

### Input Blacklisting

The framework allows for the suppression of standard game inputs (such as camera rotation or mouse clicks) while UI elements are active. This is controlled via the <xref:RichHudFramework.UI.Client.BindManager.BlacklistMode> property or <xref:RichHudFramework.UI.Client.BindManager.RequestTempBlacklist*> method using <xref:RichHudFramework.UI.SeBlacklistModes> flags.

- **Common Use Cases:** Disabling camera look while a cursor is visible, intercepting chat input for text fields, or blocking interactions with the game world while a menu is open.
- **Limitations:** Blacklisting is not comprehensive; some Space Engineers controls cannot be disabled via the Mod API. Gamepad/Joystick blacklisting functionality may vary.

> [!WARNING]
> `BindManager` coordinates blacklisting between mods using the framework to prevent conflicts. Avoid using the native [MyVisualScriptLogicProvider.SetPlayerInputBlacklistState](https://keensoftwarehouse.github.io/SpaceEngineersModAPI/api/Sandbox.Game.MyVisualScriptLogicProvider.html#Sandbox_Game_MyVisualScriptLogicProvider_SetPlayerInputBlacklistState_System_String_System_Int64_System_Boolean_) API directly, as it may interfere with this system.

---
uid: RichHudFramework.UI.Client.BindManager
example: [*content]
---

The following example demonstrates how to initialize a custom `IBindGroup`, register default key combinations using `BindGroupInitializer`, and apply previously saved configuration data.

`IBindGroup` allows binds to be retrieved by index or by name. This is convenient for initialization, but error-prone in UI integrations. For this reason, it is recommended to retrieve binds from the group once, and cache their references.

```csharp
public class MyCustomBinds
{
    // Public accessors for specific binds
    public IBind MenuOpenBind { get; private set; }
    public IBind ModifierBind { get; private set; }

    public IBindGroup BindGroup { get; private set; }

    public MyCustomBinds(IReadOnlyList<BindDefinition> savedConfig = null)
    {
        // 1. Retrieve or create the group
        BindGroup = BindManager.GetOrCreateGroup("MainControls");

        // 2. Define default binds
        var defaults = new BindGroupInitializer 
        {
            { "MenuOpen", RichHudControls.Control, RichHudControls.F },
            { "Modifier", RichHudControls.Shift }
        };

        // 3. Register defaults
        BindGroup.RegisterBinds(defaults);

        // 4. Overwrite defaults with saved config (if available)
        if (savedConfig != null)
        {
            BindGroup.TryLoadBindData(savedConfig);
        }

        // 5. Store individual bind references for use in the mod
        MenuOpenBind = BindGroup["MenuOpen"];
        ModifierBind = BindGroup["Modifier"];
    }
}
```
# Installation and Mod Integration
The RHF client should be installed in a preexisting C# mod project. See the official [Space Engineers Mod Scripting Reference](https://spaceengineers.wiki.gg/wiki/Modding/Reference/ModScripting) for details on mod creation. Installation is just a matter of copying the source into your project, and initializing the client:

1. **Install the Rich Hud Framework client:**
   - Download the latest release from [Releases](~/Releases.md).
   - Copy the `Shared` and `Client` folders to `/{ModName}/Data/Scripts/{ModName}/RichHudFramework`. The final `RichHudFramework` folder is optional but recommended for organization.

   Three client variants are available:
   - **Full** – The complete version included in the main source archive (recommended default).
   - **Term** – Minimal version containing only the Rich HUD Terminal components.
   - **Font** – Minimal version containing only the Font Manager.

2. **Initialize the client from your mod's primary session component:**
   - Add **using RichHudFramework.Client;**.
   - Call <xref:RichHudFramework.Client.RichHudClient.Init*> during the `Init` phase of your session component. Initialization may technically occur as early as `LoadData()`, but Rich HUD Master will not respond until the session's `Init` phase completes.

3. **Add Rich HUD Master as a dependency:**
   - Obtain Rich HUD Master from either the [Steam Workshop](https://steamcommunity.com/workshop/filedetails/?id=1965654081), or by downloading the latest release from [GitHub](https://github.com/ZachHembree/RichHudFramework.Master/releases) and installing manually.
   - After publishing your mod to the Workshop, add the Workshop version of Rich HUD Master to your mod’s dependency list.

>[!TIP]
> The framework is load-order independent. Your UI will function correctly regardless of whether your mod loads before or after RHM.

### Example
If your main class inherits from <xref:VRage.Game.Components.MySessionComponentBase>, a typical implementation will resemble the following:

```csharp
using RichHudFramework.Client;
using VRage.Game;
using VRage.Game.Components;

// Updating is optional for this example
[MySessionComponentDescriptor(MyUpdateOrder.NoUpdate)]
public sealed class MainModClass : MySessionComponentBase
{
    public override void Init(MyObjectBuilder_SessionComponent sessionComponent)
    {
        RichHudClient.Init(DebugName, HudInit, ClientReset);
    }

    private void HudInit()
    {
        // Registration with Rich HUD Master is now complete.
        // It is safe to begin using the framework.
    }

    public override void Draw()
    {
        if (RichHudClient.Registered)
        {
            // Perform any external framework updates here, but only after registration.
        }
    }

    private void ClientReset()
    {
        // The client has been unregistered and all framework members have stopped functioning.
        // This callback is invoked in one of three situations:
        // 1. The game session is unloading.
        // 2. An unhandled exception occurred on either the client or master side.
        // 3. RichHudClient.Reset() was called manually.
    }
}
```

In most cases the `ClientReset` callback can be left empty, as `UnloadData()` automatically cleans up all resources. Its primary purpose is to notify you of unexpected closure due to the conditions listed above.

If you are designing a reloadable mod (one that can be reloaded during an active session), call <xref:RichHudFramework.Client.RichHudClient.Reset*> to unregister the client before reinitializing.

>[!WARNING]
> When a mod defines multiple session components, **initialize the client from only one component**. Otherwise, the client will be initialized by whichever component runs `Init` first, and the `HudInit` callback will fire only for that component.
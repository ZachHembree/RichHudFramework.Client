# Custom Fonts

Custom fonts can be added to the framework by generating the required resource files, including them in your mod, and registering them via the framework's API during initialization.

**Instructions:**
1. Generate the needed resources using the [generation tool](https://github.com/ZachHembree/RichHudFontGen). 
2. Copy everything over from the root of the newly generated font folder to your mod's root. If you didn't set a mod name, you'll need to rename the folder at `{FontName}\Data\Scripts\ModName` to match the corresponding folder in your mod.
3. Once everything's copied over, make sure you've included the FontData folder in your project and add usings for `RichHudFramework.UI.Rendering.Client` and `RichHudFramework.UI.FontData`.
4. Register the new fonts in your [main class](Installation-and-Mod-Integration.md) on HudInit:

```csharp
using RichHudFramework.Client;
using RichHudFramework.UI.Rendering.Client;
using RichHudFramework.UI.FontData;
using VRage.Game;
using VRage.Game.Components;

[MySessionComponentDescriptor(MyUpdateOrder.NoUpdate)]
public sealed class MainModClass : MySessionComponentBase
{
    private static bool fontsRegistered;

...
    private void HudInit()
    {
        // Register only once
        if (!fontsRegistered)
        {
            FontManager.TryAddFont({FontName}.GetFontData());
            fontsRegistered = true;
        }
    }
...
}
```

> [!WARNING]
> Always ensure that you ahdere to the license terms of any font you use.

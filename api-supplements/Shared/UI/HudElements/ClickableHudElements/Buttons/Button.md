---
uid: RichHudFramework.UI.Button
example: [*content]
---

```csharp
var btn = new Button(parent)
{
    // Set custom texture. Donut shape.
    Material = Material.AnnulusMat,
    Color = Color.HotPink,
    // Maximize texture size without clipping or warping. Default stretches to fit.
    MatAlignment = MaterialAlignment.FitAuto,
    MouseInput =
    {
        // Custom tooltip constructed from a string using default formatting
        ToolTip = "You'll see this message when you hover over the button.",
        // Add callback invoked on click
        LeftClickedCallback = (obj, args) => MyAPIGateway.Utilities.ShowMessage(
            $"[{obj.GetType().Name}]", // Print button type name
            "You'll see this chat message when the button is clicked.")
    }
};
```
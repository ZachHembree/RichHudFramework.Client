---
uid: RichHudFramework.UI.LabelButton
example: [*content]
---

```csharp
var btn = new LabelButton(parent)
{
    // Fancy serifed font
    Format = GlyphFormat.White.WithFont("AbhayaLibreMedium"),
    // Set label text
    Text = "Clickable label, no background",
    MouseInput = 
    {
        // Custom tooltip constructed from a string using default formatting
        ToolTip = "This font doesn't look out of place at all!",
        // Add callback invoked on click
        LeftClickedCallback = (obj, args) => MyAPIGateway.Utilities.ShowMessage(
            $"[{obj.GetType().Name}]", // Print button type name
            "You'll see this chat message when the button is clicked.")
    }
};
```
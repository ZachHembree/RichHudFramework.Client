---
uid: RichHudFramework.UI.LabelBoxButton
example: [*content]
---

```csharp
var btn = new LabelBoxButton(parent)
{
    BuilderMode = TextBuilderModes.Lined, // Multi-line text
    Format = GlyphFormat.White.WithFont("Monospace"), // Chunky Monospace label
    Text = "Clickable label\nwith a background",
    Color = Color.Purple, // Purple background
    TextPadding = new Vector2(20f), // 20pt padding / 2 = 10pts on all four sides
    MouseInput = 
    {
        // Custom tooltip constructed from RichText using default background
        ToolTip = new RichText { { 
            "A tooltip with a custom font? Why not?", 
            GlyphFormat.White.WithFont("Monospace") 
        } },
        // Add callback invoked on click
        LeftClickedCallback = (obj, args) => MyAPIGateway.Utilities.ShowMessage(
            $"[{obj.GetType().Name}]", // Print button type name
            "You'll see this chat message when the button is clicked.")
    }
};
```
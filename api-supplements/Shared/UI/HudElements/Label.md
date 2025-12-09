---
uid: RichHudFramework.UI.Label
remarks: *content
---
`Label` is a non-interactive, text-only HUD element dedicated to rendering formatted <xref:RichHudFramework.UI.RichText>. It acts as a high-level wrapper, bridging the underlying text renderer with the UI graph through an internal <xref:RichHudFramework.UI.Rendering.ITextBoard>.

This UI element handles drawing text with bounding and clipping, and facilitates text manipulation through convenience properties like <xref:RichHudFramework.UI.Label.Text> and <xref:RichHudFramework.UI.Label.BuilderMode>. For direct access to the underlying rendering and formatting interface, use the <xref:RichHudFramework.UI.Label.TextBoard> property.

---
uid: RichHudFramework.UI.Label
example: [*content]
---
The following example creates a `Label` attached to <xref:RichHudFramework.UI.Client.HudMain.HighDpiRoot>, with centered text aligned to the top of the screen.

```csharp
var label = new Label(HudMain.HighDpiRoot)
{
    ParentAlignment = ParentAlignments.Top,
    Padding = new Vector2(20f),
    Format = GlyphFormat.White.WithAlignment(TextAlignment.Center),
    Text = "This unlined text will appear at the top of the screen."
};
```
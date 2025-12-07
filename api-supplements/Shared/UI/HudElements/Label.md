---
uid: RichHudFramework.UI.Label
remarks: *content
---
`Label` is a HUD element dedicated to rendering formatted <xref:RichHudFramework.UI.RichText>. It acts as a high-level wrapper for the underlying text logic, bridging the text renderer with the UI graph through an internal <xref:RichHudFramework.UI.Rendering.ITextBoard>.

This element handles drawing text with bounding and clipping. It provides full access to the underlying text interface via convenience properties like <xref:RichHudFramework.UI.Label.Text>, as well as direct access to the rendering and formatting interface through <xref:RichHudFramework.UI.Label.TextBoard>. It is a non-interactive, text-only UI element and does not render a background.

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
---
uid: RichHudFramework.UI.RichText
remarks: *content
---
`RichText` acts as a specialized, reusable string builder designed to facilitate the efficient construction and storage of formatted text. Unlike a standard <xref:System.Text.StringBuilder>, this class maintains style information—such as color, font, and alignment—associated with specific text segments.

Most text-rendering UI elements within the framework expose properties of this type or provide access to an underlying <xref:RichHudFramework.UI.Rendering.ITextBuilder> (see: [Label.Text](<xref:RichHudFramework.UI.Label.Text>) and [Label.TextBoard](<xref:RichHudFramework.UI.Label.TextBoard>)). To streamline usage, <xref:System.String> and `StringBuilder` instances can be implicitly converted to `RichText`, allowing plain text to be assigned directly to UI properties without manual conversion.

#### Formatting Behavior

Text added to a `RichText` instance without explicit formatting will inherit style instructions based on the following precedence:

* If a <xref:RichHudFramework.UI.RichText.defaultFormat> was specified during construction, that format is applied immediately.

* If no default was specified, the text remains unformatted until it is assigned to a UI element. Upon assignment, it adopts the default configuration of the target's `ITextBuilder`.

#### Collection Initializers

This class supports collection-initializer syntax. This allows developers to compose complex, multi-styled text blocks declaratively, mixing formatted strings, unformatted strings, and other `RichText` objects within a single initialization block.

> [!NOTE]
> When retrieving a `RichText` property from a UI element (e.g., a Label), the accessor returns a **deep copy** of the content currently stored in the backing `ITextBuilder`. Modifying this returned instance—via methods like `Add()`—will not automatically update the UI element. To update the display, the modified instance must be reassigned to the property.

---
uid: RichHudFramework.UI.RichText
example: [*content]
---

#### Example 1: Dynamic Text Construction
The following example shows a reused `RichText` buffer to construct a formatted string in a loop. This pattern is efficient for text that updates frequently (e.g., every frame in `HandleInput` or `Layout`).

```csharp
private static readonly GlyphFormat headerFormat = new GlyphFormat(textSize: 1.5f, alignment: TextAlignment.Center);
private static readonly GlyphFormat footerFormat = new GlyphFormat(textSize: 0.8f, color: Color.LightGoldenrodYellow);

private readonly Label multilineLabel;
private readonly RichText buffer = new RichText();

protected override void HandleInput(Vector2 cursorPos)
{
    // Clear the buffer to reuse the internal StringBuilders
    buffer.Clear();

    for (int i = 0; i < 10; i++)
    {
        // Add text with specific styles
        buffer.Add("This is a formatted header\n", headerFormat);
        buffer.Add("This is body text that will receive defaulted formatting.\n");
        buffer.Add("This is a formatted footer\n", footerFormat);
    }

    // Reassigning the buffer updates the UI element's text board
    multilineLabel.Text = buffer;
}
```

#### Example 2: Declarative Styling
This example demonstrates how to use collection initializers to create a static, multi-styled text block. This approach is ideal for help text, tooltips, or static UI labels.

```csharp
// Define reusable formats
var headerFormat = new GlyphFormat(textSize: 1.5f, alignment: TextAlignment.Center);
var subheaderFormat = new GlyphFormat(textSize: 1.1f, color: Color.LightGoldenrodYellow);
var bodyFormat = new GlyphFormat(textSize: 0.9f);

// Initialize RichText with a default body format
var text = new RichText(bodyFormat) 
{
    // 1. Add text with specific overriding formats (Header)
    { "Header for a Multi-Line Text Block\n", headerFormat },

    // 2. Add plain strings (inherits bodyFormat from constructor)
    "Multi-line body text using bodyFormat via defaultFormat.\n", 
    "Strings, StringBuilders, or other RichText objects are appended via dedicated Add() overloads.\n",

    // 3. Add a subsection header using the specific format
    { "Subsection: This is using subheaderFormat\n", subheaderFormat },

    // 4. Continue adding body text
    "Multi-line text requires the ITextBuilder or Label being written to to be in ",
    "the correct mode (Lined or Wrapped) to accept manual line breaks.\n"
};
```
---
uid: RichHudFramework.UI.Client.TextPage
example: [*content]
---
This example creates a `TextPage` configures the headers, and sets a short body example with custom text formatting for a subsection header.

```csharp
var textPage = new TextPage
{
    // Name in sidebar listing
    Name = "Text Page Name",
    // Contents
    HeaderText = "This is a one-line rich text header",
    SubHeaderText = "This is a one-line rich text subheader",
    Text = new RichText 
    {
        { "This is a wrapped rich text body that can be arbitrarily long, using default formatting..." },
        { "\n\nThis is a subsection header", GlyphFormat.White
            .WithColor(Color.LightGoldenrodYellow)
            .WithStyle(FontStyles.Underline) }
    }
};
RichHudTerminal.Root.Add(textPage);
```
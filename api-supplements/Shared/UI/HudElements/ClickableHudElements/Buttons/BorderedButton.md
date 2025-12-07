---
uid: RichHudFramework.UI.BorderedButton
example: [*content]
---

This will create a button with a border, very similar in appearance to those found in Space Engineer's terminal, but with a bright orange tooltip and an exceptionally garish pink-purple focus format, because why not?

By default, the focus formatting matches the default Space Engineers behavior: light blue background with dark text.

```csharp
var btn = new BorderedButton(parent)
{
    Text = "Send Chat Message",
    BorderColor = Color.Green, // Your button / your rules
    // This is going to really pop when you click it
    FocusTextColor = Color.LightPink,
    FocusColor = Color.Purple,
    MouseInput =
    {
        // Custom tooltip constructed from RichText with custom background
        ToolTip = new ToolTip 
        {
            text = "This is a bright orange warning tooltip.",
            bgColor = Color.OrangeRed
        },
        // Add callback invoked on click
        LeftClickedCallback = (obj, args) => MyAPIGateway.Utilities.ShowMessage(
            $"[{obj.GetType().Name}]", // Print button type name
            "You'll see this chat message when the button is clicked.")
    }
};
```
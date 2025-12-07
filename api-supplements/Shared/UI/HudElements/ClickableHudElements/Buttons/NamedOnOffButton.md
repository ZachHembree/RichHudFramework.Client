---
uid: RichHudFramework.UI.NamedOnOffButton
example: [*content]
---

This example creates a labeled on/off toggle with a <xref:RichHudFramework.UI.ColorPickerHSV> below it, whose input is enabled or disabled according to the toggle.

```csharp
new HudChain(parent)
{
    AlignVertical = true, // Create a vertical toggle + HSV picker stack
    Spacing = 10f,
    CollectionContainer = 
    {
        new NamedOnOffButton
        {
            Name = "Color picker input toggle",
            UpdateValueCallback = (obj, args) => 
            {
                var toggle = (NamedOnOffButton)obj;
                var chain = (HudChain)toggle.Parent;
                // Get the color picker below and set its input state
                chain[1].Element.InputEnabled = toggle.Value;
            },
            MouseInput = 
            {
                ToolTip = "This toggle enables/disables the color picker's input."
            }
        },
        new ColorPickerHSV()
    }
};
```
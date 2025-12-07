---
uid: RichHudFramework.UI.NamedSliderBox
example: [*content]
---

This example demonstrates the correct way to update the <xref:RichHudFramework.UI.NamedSliderBox.ValueText> of a `NamedSliderBox`.

```csharp
new NamedSliderBox(parent)
{
    Name = "Some angle",
    Min = -360f, Max = 360f,
    MouseInput = 
    {
        ToolTip = $"This {nameof(NamedSliderBox)} could set an angle for something, or something."
    },
    // Value text is not set automatically. This allows for custom formatting, but
    // requires some additional setup.
    UpdateValueCallback = (obj, args) =>
    {
        var slider = (NamedSliderBox)obj;
        slider.ValueBuilder.SetText($"{slider.Value:F1}°");
    }
};
```
---
uid: RichHudFramework.UI.Client.TerminalSlider
example: [*content]
---

```csharp
new TerminalSlider()
{
    Name = "TerminalSlider",
    ValueText = "0",
    Min = 0f, Max = 100f, Value = 0f,
    ControlChangedHandler = (obj, args) =>
    {
        var slider = obj as TerminalSlider;
        slider.ValueText = $"{slider.Value:G4}";
        ExceptionHandler.SendChatMessage($"Slider value changed: {slider.Value:G3}");
    },
    ToolTip = "Slider based on 32-bit float value with customizable value text."
}
```
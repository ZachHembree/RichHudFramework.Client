---
uid: RichHudFramework.UI.Client.ControlCategory
example: [*content]
---

```csharp
new ControlCategory()
{
    HeaderText = "ControlCategory",
    SubheaderText = "Contains terminal controls grouped into ControlTiles",
    TileContainer =
    {
        new ControlTile()
        {
            new TerminalOnOffButton()
            {
                Name = "TerminalOnOffButton",
                ControlChangedHandler = (obj, args) =>
                {
                    var toggle = obj as TerminalOnOffButton;
                    ExceptionHandler.SendChatMessage($"{toggle.Name} = {toggle.Value}");
                },
                ToolTip = "Alternative to checkbox. Functionally identical."
            },
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
            },
        },
    }
}
```
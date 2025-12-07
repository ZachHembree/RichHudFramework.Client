---
uid: RichHudFramework.UI.Client.TerminalColorPicker
example: [*content]
---

```csharp
new TerminalColorPicker()
{
    Name = "TerminalColorPicker",
    ControlChangedHandler = (obj, args) =>
    {
        var picker = obj as TerminalColorPicker;
        ExceptionHandler.SendChatMessage($"Color = ({picker.Value.R}, {picker.Value.G}, {picker.Value.B})");
    },
    ToolTip = "Sets a simple 24-bit RGB color, 0-255/8-bits per channel."
}
```
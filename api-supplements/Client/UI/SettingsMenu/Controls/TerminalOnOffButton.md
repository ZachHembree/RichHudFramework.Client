---
uid: RichHudFramework.UI.Client.TerminalOnOffButton
example: [*content]
---

```csharp
new TerminalOnOffButton()
{
    Name = "TerminalOnOffButton",
    ControlChangedHandler = (obj, args) =>
    {
        var toggle = obj as TerminalOnOffButton;
        ExceptionHandler.SendChatMessage($"{toggle.Name} = {toggle.Value}");
    },
    ToolTip = "Alternative to checkbox. Functionally identical."
}
```
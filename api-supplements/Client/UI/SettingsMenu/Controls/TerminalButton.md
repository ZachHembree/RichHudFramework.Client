---
uid: RichHudFramework.UI.Client.TerminalButton
example: [*content]
---

```csharp
new TerminalButton()
{
    Name = "TerminalButton",
    ControlChangedHandler = (obj, args) =>
    {
        var btn = obj as TerminalButton;
        ExceptionHandler.SendChatMessage($"{btn.Name} pressed");
    },
    ToolTip = "Simple button. Fires an event when clicked."
}
```
---
uid: RichHudFramework.UI.Client.TerminalCheckbox
example: [*content]
---

```csharp
new TerminalCheckbox()
{
    Name = "TerminalCheckbox",
    ControlChangedHandler = (obj, args) =>
    {
        var checkbox = obj as TerminalCheckbox;
        ExceptionHandler.SendChatMessage($"{checkbox.Name} = {checkbox.Value}");
    },
    ToolTip = "Sets a binary value. Fires an event when toggled."
}
```
---
uid: RichHudFramework.UI.Client.TerminalTextField
example: [*content]
---

```csharp
new TerminalTextField()
{
    Name = "TerminalTextField",
    ControlChangedHandler = (obj, args) =>
    {
        var field = obj as TerminalTextField;
        ExceptionHandler.SendChatMessage($"New text: {field.Value}");
    },
    ToolTip = "One-line text field"
}
```
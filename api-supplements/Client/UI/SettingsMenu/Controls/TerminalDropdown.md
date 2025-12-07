---
uid: RichHudFramework.UI.Client.TerminalDropdown`1
example: [*content]
---

```csharp
new TerminalDropdown<float>()
{
    Name = "TerminalDropdown",
    List =
    {
        { "Entry 1", 0f },
        { "Entry 2", 1f },
        { "Entry 3", 1f },
        { "Entry 4", 2f },
        { "Entry 5", 3f },
        { "Entry 6", 5f },
        { "Entry 7", 8f },
        { "Entry 8", 13f },
    },
    ControlChangedHandler = (obj, args) =>
    {
        var list = obj as TerminalDropdown<float>;
        ExceptionHandler.SendChatMessage($"Selected: {list.Value.Element.TextBoard} = {list.Value.AssocMember}");
    },
    ToolTip = "A generic dropdown list with custom labels associated with arbitrary values."
}
```
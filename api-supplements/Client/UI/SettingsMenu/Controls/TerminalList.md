---
uid: RichHudFramework.UI.Client.TerminalList`1
example: [*content]
---

```csharp
new TerminalList<float>()
{
    Name = "TerminalList",
    List =
    {
        { "Entry 1", 21f },
        { "Entry 2", 34f },
        { "Entry 3", 55f },
        { "Entry 4", 89f },
        { "Entry 5", 144f },
        { "Entry 6", 233f },
        { "Entry 7", 377f },
        { "Entry 8", 610f },
    },
    ControlChangedHandler = (obj, args) =>
    {
        var list = obj as TerminalList<float>;
        ExceptionHandler.SendChatMessage($"Selected: {list.Value.Element.TextBoard} = {list.Value.AssocMember}");
    },
    ToolTip = "Fixed-size scrolling list with custom labels associated with arbitrary values."
}
```
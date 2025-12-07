---
uid: RichHudFramework.UI.Client.TerminalDragBox
example: [*content]
---

```csharp
new TerminalDragBox()
{
    Name = "TerminalDragBox",
    ControlChangedHandler = (obj, args) =>
    {
        var box = obj as TerminalDragBox;
        ExceptionHandler.SendChatMessage($"New box position: {box.Value}");
    },
    ToolTip = "Spawns a draggable window for setting fixed position on the HUD.\nUseful for user-configurable" +
    " UI layout."
}
```
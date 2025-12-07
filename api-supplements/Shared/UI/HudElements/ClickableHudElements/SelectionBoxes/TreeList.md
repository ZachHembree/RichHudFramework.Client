---
uid: RichHudFramework.UI.TreeList`3
remarks: *content
---
There are two aliases for `TreeList`s used to simplify common usages:
- <xref:RichHudFramework.UI.TreeList`2>: Defaults `TContainer` to <xref:RichHudFramework.UI.ListBoxEntry`2>.
- <xref:RichHudFramework.UI.TreeList`1>: Defaults `TContainer` and `TElement` to <xref:RichHudFramework.UI.ListBoxEntry`2> and <xref:RichHudFramework.UI.Label>, respectively.

---
uid: RichHudFramework.UI.TreeList`3
example: [*content]
---

```csharp
new TreeList<string>(parent)
{
    Name = "Custom TreeList Label",
    UpdateValueCallback = (obj, args) =>
    {
        var treeList = (TreeList<string>)obj;
        MyAPIGateway.Utilities.ShowMessage(
            $"[{treeList.GetType().Name}]",
            $"Selected: {treeList.Value.AssocMember}");
    },
    ListContainer = 
    {
        { "Label 1", "Key 1" },
        { "Label 2", "Key 2" },
        { "Label 3", "Key 3" },
        { "Label 4", "Key 4" },
        { "Label 5", "Key 5" },
        { "Label 6", "Key 6" }
    }
};
```
---
uid: RichHudFramework.UI.TreeBox`2
remarks: *content
---
<xref:RichHudFramework.UI.TreeBox`1> is used as an alias to simplify usage by defaulting `TContainer` and `TElement` to <xref:RichHudFramework.UI.SelectionBoxEntryTuple`2> and <xref:RichHudFramework.UI.LabelElementBase>, respectively.

---
uid: RichHudFramework.UI.TreeBox`2
example: [*content]
---

`TreeBox` is a more generalized version of <xref:RichHudFramework.UI.TreeList`2>. It trades uniform, pooled entries for entries of any type that extend <xref:RichHudFramework.UI.LabelElementBase>.

As seen below, this means additional setup is required for normal entries, but it also allows for heterogeneous entries. Labels, dropdowns, other TreeBoxes or custom elements can all be added to the same list.

```csharp
private static Label GetCustomLabel(string name)
{
    return new Label
    {
        Format = GlyphFormat.Blueish,
        Text = name,
        AutoResize = false,
        Height = 28f,
        Padding = new Vector2(20f, 0f)
    };
}

new TreeBox<string>(parent)
{
    Name = "Custom TreeBox Label",
    ParentAlignment = ParentAlignments.InnerTop,
    UpdateValueCallback = (obj, args) =>
    {
        var treeList = (TreeBox<string>)obj;
        MyAPIGateway.Utilities.ShowMessage(
            $"[{treeList.GetType().Name}]",
            $"Selected: {treeList.Value.AssocMember}");
    },
    ListContainer =
    {
        { GetCustomLabel("Label 1"), "Key 1" },
        { GetCustomLabel("Label 2"), "Key 2" },
        { GetCustomLabel("Label 3"), "Key 3" },
        { GetCustomLabel("Label 4"), "Key 4" },
        {
            new TreeList<string>
            {
                Name = "Nested TreeList (5)",
                ParentAlignment = ParentAlignments.InnerTop,
                UpdateValueCallback = (obj, args) =>
                {
                    var treeList = (TreeList<string>)obj;
                    MyAPIGateway.Utilities.ShowMessage(
                        $"Nested [{treeList.GetType().Name}]",
                        $"Selected: {treeList.Value.AssocMember}");
                },
                ListContainer =
                {
                    { "Label 6", "Key 6" },
                    { "Label 7", "Key 7" },
                    { "Label 8", "Key 8" },
                    { "Label 9", "Key 9" },
                }
            },
            "Key 5", // Key for the nested tree
            false // Disable highlighting for the inner tree. It does its own highlighting.
        }
    }
};
```
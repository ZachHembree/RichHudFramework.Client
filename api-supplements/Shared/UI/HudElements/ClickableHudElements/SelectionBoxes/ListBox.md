---
uid: RichHudFramework.UI.ListBox`3
remarks: *content
---
<xref:RichHudFramework.UI.ListBox`1> is used as an alias to simplify common usages by defaulting `TContainer` and `TElement` to <xref:RichHudFramework.UI.ListBoxEntry`2> and <xref:RichHudFramework.UI.Label>, respectively.

---
uid: RichHudFramework.UI.ListBox`3
example: [*content]
---

```csharp
new ListBox<Color>(parent)
{
    // Set background color to selection value
    UpdateValueCallback = (obj, args) =>
    {
        var listBox = (ListBox<Color>)obj;
        MyAPIGateway.Utilities.ShowMessage(
            $"[{listBox.GetType().Name}]",
            $"Key: {listBox.Value.Element.Text} - Value: {listBox.Value.AssocMember}");

        listBox.Color = listBox.Value.AssocMember;
    },
    ListContainer =
    {
        { "Color 1", Color.Red },
        { "Color 2", Color.Plum },
        { "Color 3", Color.Green },
        { "Default", TerminalFormatting.DarkSlateGrey }
    }
}
```
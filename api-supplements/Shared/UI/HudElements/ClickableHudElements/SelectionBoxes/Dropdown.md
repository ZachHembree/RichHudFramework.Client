---
uid: RichHudFramework.UI.Dropdown`3
remarks: *content
---
There are two aliases for `Dropdown`s used to simplify common usages:
- <xref:RichHudFramework.UI.Dropdown`2>: Defaults `TContainer` to <xref:RichHudFramework.UI.ListBoxEntry`2>.
- <xref:RichHudFramework.UI.Dropdown`1>: Defaults `TContainer` and `TElement` to <xref:RichHudFramework.UI.ListBoxEntry`2> and <xref:RichHudFramework.UI.Label>, respectively.

---
uid: RichHudFramework.UI.Dropdown`3
example: [*content]
---

```csharp
var dropdown = new Dropdown<int>(parent)
{
    // Automatically expand to include at least four visible entries at a time, 
    // if there are that many.
    MinVisibleCount = 4,
    UpdateValueCallback = (obj, args) =>
    {
        var sender = (Dropdown<int>)obj;
        MyAPIGateway.Utilities.ShowMessage(
            $"[{sender.GetType().Name}]",
            $"Key: {sender.Value.Element.Text} - Value: {sender.Value.AssocMember}");
    },
    ListContainer =
    {
        { "Label 1", 1234 },
        { new RichText("Label 2", GlyphFormat.White.WithColor(Color.Red)), 4567 },
        { new RichText("Label 3", GlyphFormat.White.WithColor(Color.Green)), 8910 },
        { new RichText("Label 4", GlyphFormat.White.WithColor(Color.GreenYellow)), 1112 },
        { "Label 5", 1314 },
        { "Label 6", 1516 },
        { "Label 7", 1718 },
        { "Label 8", 1920 }
    }
};
var label = new Label(dropdown)
{
    Text = "Dropdown Label",
    ParentAlignment = ParentAlignments.InnerLeft | ParentAlignments.Top
};
```
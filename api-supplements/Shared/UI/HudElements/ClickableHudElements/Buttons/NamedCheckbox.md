---
uid: RichHudFramework.UI.NamedCheckBox
example: [*content]
---

```csharp
new NamedCheckBox(parent)
{
    Name = "This checkbox has a label",
    // Write value to chat when it changes
    UpdateValueCallback = (obj, args) =>
        MyAPIGateway.Utilities.ShowMessage(
        $"[{obj.GetType().Name}]", // Print checkbox type
        $"Checkbox Value: {((NamedCheckBox)obj).Value}"),
    // Add a custom tooltip
    MouseInput =
    {
        ToolTip = "Click on this to change the value of the checkbox."
    }
};
```
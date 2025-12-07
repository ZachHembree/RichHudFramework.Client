---
uid: RichHudFramework.UI.BorderedCheckBox
example: [*content]
---

This creates a checkbox with a <xref:RichHudFramework.UI.Label> as a separate element, organized into a horizontal row with a <xref:RichHudFramework.UI.HudChain>. The visibility of the label is set equal to the value of the checkbox when it changes.

```csharp
new HudChain(parent)
{
    // We want the container to grow to fit, but we don't want it
    // shrinking if we hide the label.
    SizingMode = HudChainSizingModes.ClampChainBoth,
    Spacing = 10f,
    CollectionContainer = 
    {
        // Checkbox without a label
        new BorderedCheckBox
        {
            // Show/hide label when ticked/unticked
            UpdateValueCallback = (obj, args) =>
            {
                var checkBox = (BorderedCheckBox)obj;
                var chain = (HudChain)checkBox.Parent;
                // Get the label after this checkbox in the chain
                chain[1].Element.Visible = checkBox.Value;
            },
            // Add a custom tooltip
            MouseInput =
            {
                ToolTip = "This shows/hides the label."
            }
        },
        // Add a label to the right of the checkbox
        new Label()
    }
};
```
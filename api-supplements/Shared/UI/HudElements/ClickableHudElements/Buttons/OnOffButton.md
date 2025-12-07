---
uid: RichHudFramework.UI.OnOffButton
example: [*content]
---

This creates an unlabeled on/off toggle with custom label names, and prints the label corresponding to the selected value to chat when changed.

```csharp
new OnOffButton(parent)
{
    OnText = "Mode 1",
    OffText = "Mode 2",
    UpdateValueCallback = (obj, args) => 
    {
        var toggle = (OnOffButton)obj;
        // Get mode string from boolean value
        string valStr = toggle.Value ? 
            toggle.OnText.ToString() : 
            toggle.OffText.ToString();
        // Write value to chat
        MyAPIGateway.Utilities.ShowMessage(
            $"[{toggle.GetType().Name}]", // Print toggle type
            $"Toggle Value: {valStr}");
    },
    MouseInput = 
    { 
        ToolTip = "This unlabeled toggle prints the mode name to chat."
    }
};
```
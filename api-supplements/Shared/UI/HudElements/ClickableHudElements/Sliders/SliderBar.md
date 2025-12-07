---
uid: RichHudFramework.UI.SliderBar
example: [*content]
---

```csharp
new SliderBar(parent)
{
    Min = 100, Max = 1000, Value = 450,
    BarSize = new Vector2(200f, 20f),
    SliderSize = new Vector2(50f, 30f),
    UpdateValueCallback = (obj, args) =>
    {
        var slider = (SliderBar)obj;
        MyAPIGateway.Utilities.ShowMessage(
            $"[{slider.GetType().Name}]",
            $"Value: {slider.Value:F1} ({slider.Percent:P1})"
        );
    },
    ToolTip = "Slider input base type. No labels or styling."
};
```
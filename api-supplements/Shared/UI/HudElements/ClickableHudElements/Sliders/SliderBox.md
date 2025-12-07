---
uid: RichHudFramework.UI.SliderBox
example: [*content]
---

```csharp
new SliderBox(parent)
{
    Min = 0f, Max = 1f, Value = 0.5f,
    MouseInput = 
    {
        ToolTip = $"This {nameof(SliderBox)} sets its own background opacity."
    },
    UpdateValueCallback = (obj, args) =>
    {
        var slider = (SliderBox)obj;
        slider.BackgroundColor = slider.BackgroundColor.SetAlphaPct(slider.Percent);
    }
};
```
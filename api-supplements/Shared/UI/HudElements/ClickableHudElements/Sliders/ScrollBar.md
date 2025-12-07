---
uid: RichHudFramework.UI.ScrollBar
example: [*content]
---

This example creates a horizontal `ScrollBar` with a <xref:RichHudFramework.UI.Label> attached to the bottom, whose horizontal offset is set by the slider.

```csharp
var scrollLabel = new Label
{
    // Align top edge to to ScrollBar's bottom edge
    ParentAlignment = ParentAlignments.Bottom,
    BuilderMode = TextBuilderModes.Lined, // Multi-line text
    Text = "This label gets\ndragged by the slider."
};
var scrollBar = new ScrollBar(parent)
{
    Vertical = false,
    UnpaddedSize = new Vector2(300, 30f),
    // Keep text inside the left/right bounds
    Min = -150f + 0.5f * scrollLabel.TextBoard.TextSize.X,
    Max = 150f - 0.5f * scrollLabel.TextBoard.TextSize.X,
    Percent = 0.5f,
    SlideInput =
    {
        BarColor = Color.Pink,
        SliderColor = Color.Green,
        ToolTip = $"This {nameof(ScrollBar)} drags the text below."
    },
    // Drag text
    UpdateValueCallback = (obj, args) =>
    {
        var sender = (ScrollBar)obj;
        scrollLabel.Offset = new Vector2(sender.Value, 0f);
    }
};
scrollLabel.Register(scrollBar);
```
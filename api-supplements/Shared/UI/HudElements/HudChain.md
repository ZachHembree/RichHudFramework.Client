---
uid: RichHudFramework.UI.HudChain`2
remarks: *content
---

This class extends [HudCollection](xref:RichHudFramework.UI.HudCollection`2) and serves as a fundamental layout tool that organizes child <xref:RichHudFramework.UI.HudElementBase> elements into a linear stack, either vertically or horizontally. Conceptually similar to a CSS Flexbox or a XAML StackPanel, it provides the underlying logic for automatically arranging UI elements without manual pixel positioning.

#### Layout and Sizing

The orientation of the stack is controlled by the <xref:RichHudFramework.UI.HudChain`2.AlignVertical> property. In the framework's terminology, the direction of the stack is the **Align Axis** (Y for vertical, X for horizontal), while the perpendicular direction is the **Off Axis**. Layout behavior is further defined by <xref:RichHudFramework.UI.HudChainSizingModes>, which determines whether the chain expands to fit its members or clamps them to its own fixed size. Individual elements can be set to fixed sizes or configured to scale proportionally to fill remaining space using the <xref:RichHudFramework.UI.IChainElementContainer`1.AlignAxisScale> property on their containers.

#### Usage and Derivatives

While this generic class allows for full customization of container types, the framework includes convenience aliases—<xref:RichHudFramework.UI.HudChain`1> and <xref:RichHudFramework.UI.HudChain>—which utilize default container and element types for standard use cases. This infrastructure also forms the foundation for scrollable lists, such as <xref:RichHudFramework.UI.ScrollBox`2>, and complex controls like <xref:RichHudFramework.UI.SelectionBoxBase`3>. 

> [!TIP]
> Although the framework does not include a native grid container, complex 2D layouts can be achieved by nesting vertical and horizontal chains.

---
uid: RichHudFramework.UI.HudChain`2
example: [*content]
---

The following example demonstrates how to create a horizontal row (chain) containing a label, a button, and a dropdown. The label retains its natural size, while the button and dropdown are configured to split the remaining horizontal width equally.

```csharp
var row = new HudChain(parent)
{
    // Make chain horizontal
    AlignVertical = false,
    // Define the external bounds and spacing
    UnpaddedSize = new Vector2(300f, 40f),
    Padding = new Vector2(10f),
    Spacing = 8f,
    // Force members to match the chain's height (Off Axis) 
    // and allow the chain to calculate width distribution (Align Axis).
    SizingMode = HudChainSizingModes.FitMembersOffAxis,
    CollectionContainer = 
    {
        // Element 1: Label
        // AlignAxisScale is 0f: It keeps its calculated width.
        { new Label { Text = "Row Label" }, 0f }, 
        // Element 2: Button
        // AlignAxisScale is 0.5f: It takes 50% of the *remaining* space.
        { new BorderedButton { Text = "Action" }, 0.5f }, 
        // Element 3: Dropdown
        // AlignAxisScale is 0.5f: It takes the other 50% of remaining space.
        { new Dropdown<int>
        {
            ListContainer =
            {
                { "Mode 1", 1 },
                { "Mode 2", 2 },
                { "Mode 3", 3 }
            }
        }, 0.5f }
    }
};
```
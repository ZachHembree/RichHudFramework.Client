---
uid: RichHudFramework.UI.HudElementBase
remarks: *content
---

This class serves as the foundational base class for UI elements within the framework, providing essential functionality for sizing, positioning, hierarchy management, and input interaction. While lighter base classes exist in the node graph, this class represents the minimum requirement for any element that requires definite dimensions and offsets.

### Parenting and Registration
Every UI element must be registered to a parent node to be updated, rendered, or receive input. Registration is normally performed automatically by the constructor, or manually via <xref:RichHudFramework.UI.HudNodeBase.Register*>.

- For an element to be rendered or process input, its hierarchy must ultimately trace back to one of the global root nodes provided by <xref:RichHudFramework.UI.Client.HudMain#RichHudFramework_UI_Client_HudMain_remarks>.
- Visibility and input handling for registered nodes can be toggled dynamically using <xref:RichHudFramework.UI.HudParentBase.Visible> and <xref:RichHudFramework.UI.HudParentBase.InputEnabled>.

### Update Cycle and Customization
This class extends <xref:RichHudFramework.UI.HudNodeBase> and <xref:RichHudFramework.UI.HudParentBase>, exposing the core update loop hooks required to create custom UI behavior imperatively:

1. <xref:RichHudFramework.UI.HudParentBase.Measure*>: Logic for calculating element dimensions for self-resizing (occurring before the main layout pass).
2. <xref:RichHudFramework.UI.HudParentBase.Layout*>: Logic for arranging child elements, setting finalized sizes, and updating relative positioning.
3. <xref:RichHudFramework.UI.HudParentBase.Draw*>: Logic for rendering the element (e.g., drawing billboards or text).
4. <xref:RichHudFramework.UI.HudParentBase.InputDepth*> and <xref:RichHudFramework.UI.HudParentBase.HandleInput*>: Logic for cursor and keyboard polling.

> [!NOTE]
> Parent nodes can override the sizing of child nodes during layout. Custom UI elements should be designed to respect overrides where practical to ensure stable and correct positioning.

### Layout and Alignment
By default, UI elements are placed relative to the center of the parent node; their <xref:RichHudFramework.UI.HudElementBase.Origin>s are centered. This differs from many UI systems that default to top-left. For UI elements attached directly to one of the root nodes, their position will be relative to the center of the screen. Positioning is set via <xref:RichHudFramework.UI.HudElementBase.Offset>.

This class includes declarative utilities to simplify layout management without requiring manual calculations in the `Layout` hook:

* <xref:RichHudFramework.UI.HudElementBase.ParentAlignment>: Configures the anchoring of the element's `Origin` relative to its parent. When attached directly to `Root` or `HighDpiRoot`, this alignment is clamped to keep the element within screen bounds.

* <xref:RichHudFramework.UI.HudElementBase.DimAlignment>: Configures automatic size matching. Elements can be set to match their parent's width, height, or both (with or without padding).

By combining these alignment properties with container types like <xref:RichHudFramework.UI.HudChain`2>, complex UI layouts can be constructed without resorting to writing manual layout logic.

### Input Handling
Input can be polled manually, using the `HandleInput` method seen above, but most standard UI elements have events for value changes, and mouse input events via <xref:RichHudFramework.UI.IMouseInput> components. <xref:RichHudFramework.UI.BindInputElement> provides events for registering custom <xref:RichHudFramework.UI.IBind> presses (created with <xref:RichHudFramework.UI.Client.BindManager>) within the context of the UI node. This allows most common input handling to be implemented using only event-driven logic.

---
uid: RichHudFramework.UI.HudElementBase
example: [*content]
---

The following example demonstrates how to create a custom UI element by extending this class. It implements a loading bar with an infinitely looping animation using two <xref:RichHudFramework.UI.TexturedBox> components. The background box uses masking to clip the foreground bar as it slides continuously from left to right.

```csharp
public class LoadingBarExample : HudElementBase
{
    private float animStep;
    private readonly TexturedBox foreground, background;

    // Recommended constructor format
    public LoadingBarExample(HudParentBase parent = null) : base(parent)
    {
        background = new TexturedBox(this)
        {
            Color = TerminalFormatting.DarkSlateGrey, // Dark background
            DimAlignment = DimAlignments.UnpaddedSize,
            IsMasking = true // Clip contents that slide out of bounds
        };

        foreground = new TexturedBox(background)
        { 
            Color = TerminalFormatting.Mercury, // Bright foreground
            DimAlignment = DimAlignments.Height,
            ParentAlignment = ParentAlignments.InnerLeft
        };

        // Set default size and padding
        UnpaddedSize = new Vector2(200f, 30f);
        Padding = new Vector2(20f);
        animStep = 0f;
    }

    protected override void Layout()
    {
        // Update animation state
        animStep = Math.Abs(animStep + 1E-2f) % 1.3f;

        // Update position based on animation step
        foreground.Width = background.Width * 0.3f;
        foreground.Offset = new Vector2(background.Width * animStep - foreground.Width, 0f);
    }
}
```
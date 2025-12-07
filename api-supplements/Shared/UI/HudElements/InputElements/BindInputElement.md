---
uid: RichHudFramework.UI.BindInputElement
remarks: *content
---
`BindInputElement` functions as a specialized logic node that bridges <xref:RichHudFramework.UI.IBind> input with the UI system. Events triggered by this element (such as <xref:RichHudFramework.UI.IBindEventProxy.NewPressed> or <xref:RichHudFramework.UI.IBindEventProxy.PressedAndHeld>) will only fire if the parent UI node is currently <xref:RichHudFramework.UI.HudParentBase.Visible> and has <xref:RichHudFramework.UI.HudParentBase.InputEnabled> set to true. This ensures that keybinds do not trigger actions when the UI is hidden or disabled.

---
uid: RichHudFramework.UI.BindInputElement
example: [*content]
---
This example demonstrates how to use a `BindInputElement` to add keyboard scrolling to a <xref:RichHudFramework.UI.SliderBar>. Collection initializer syntax is used to map the <xref:RichHudFramework.UI.SharedBinds.LeftArrow> and <xref:RichHudFramework.UI.SharedBinds.RightArrow> directly to handler methods.

```csharp
public static void CreateSlider(HudParentBase parent)
{
    var slider = new SliderBar(parent)
    {
        Vertical = false,
        Min = 0f, Max = 100f, Value = 50f
    };

    var input = new BindInputElement(slider)
    {
        // Declarative registration of binds and callbacks
        CollectionInitializer = 
        {
            // Format: { IBind, NewPressed, PressedAndHeld, Released (null) }
            { SharedBinds.LeftArrow, OnSliderLeftArrow, OnSliderLeftArrow },
            { SharedBinds.RightArrow, OnSliderRightArrow, OnSliderRightArrow }
        }
    };
}

private static void OnSliderLeftArrow(object sender, EventArgs e)
{
    // The sender is the InputOwner (the SliderBar), not the BindInputElement
    var slider = (SliderBar)sender;
    slider.Value -= 1f;
}

private static void OnSliderRightArrow(object sender, EventArgs e)
{
    var slider = (SliderBar)sender;
    slider.Value += 1f;
}
```

#### Alternate Initialization

This demonstrates an alternative initialization pattern. Instead of using the collection initializer, the specific <xref:RichHudFramework.UI.IBindEventProxy> objects are retrieved using the class indexer. This approach is useful when adding binds dynamically after the object has been constructed or when more explicit syntax is desired.

```csharp
var input = new BindInputElement(slider) { IsFocusRequired = false };

// Retrieve the event proxy for the Left Arrow bind
IBindEventProxy leftProxy = input[SharedBinds.LeftArrow];

// Subscribe to events
leftProxy.NewPressed += OnSliderLeftArrow;
leftProxy.PressedAndHeld += OnSliderLeftArrow;

// Retrieve and subscribe for the Right Arrow bind
IBindEventProxy rightProxy = input[SharedBinds.RightArrow];
rightProxy.NewPressed += OnSliderRightArrow;
rightProxy.PressedAndHeld += OnSliderRightArrow;
```
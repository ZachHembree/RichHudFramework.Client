---
uid: RichHudFramework.UI.ScrollBox`2
remarks: *content
---
`ScrollBox` is a scrollable variation of <xref:RichHudFramework.UI.HudChain`2>. It maintains the ability to organize child elements into a linear stack—either vertically or horizontally—while constraining them within a fixed viewport. Content that exceeds the defined bounds is clipped by default, and a scrollbar is automatically provided to facilitate navigation through the collection.

Unlike a standard chain, this container requires that elements be wrapped in <xref:RichHudFramework.UI.IScrollBoxEntry`1> objects. This interface introduces an `Enabled` state to the container, allowing specific entries to be dynamically hidden or shown within the scrolling list without creating layout gaps or removing them from the underlying collection. This makes the control particularly suitable for filtered lists or large collections of data.

#### Layout and Sizing

The control fully supports per-element proportional scaling using <xref:RichHudFramework.UI.IChainElementContainer`1.AlignAxisScale> and automatic sizing behaviors via <xref:RichHudFramework.UI.HudChain`2.SizingMode>. To determine the visible area of the list, developers should utilize <xref:RichHudFramework.UI.ScrollBox`2.MinLength> or <xref:RichHudFramework.UI.ScrollBox`2.MinVisibleCount>, or manually assign a fixed size to the element.

#### Usages

This class serves as the foundational layout and storage mechanism for higher-level selection controls, including <xref:RichHudFramework.UI.ListBox`3> and <xref:RichHudFramework.UI.Dropdown`3>.

---
uid: RichHudFramework.UI.ScrollBox`2
example: [*content]
---
The following example demonstrates how to create a fixed-size `ScrollBox` attached to <xref:RichHudFramework.UI.Client.HudMain.HighDpiRoot>. It configures the container to stack elements vertically and populates it with a variety of standard UI controls, including buttons, sliders, text fields, and a nested list box.

```csharp
var sidebar = new ScrollBox(HudMain.HighDpiRoot)
{
    // Create a vertical scrolling list
    AlignVertical = true,
    // Match member width to ScrollBox width
    SizingMode = HudChainSizingModes.FitMembersOffAxis,
    // Align to right side of the screen
    ParentAlignment = ParentAlignments.Right,
    // Set fixed size
    Size = new Vector2(300, 400),
    Padding = new Vector2(40),
    Spacing = 10,
    // Nearly anything can go in here
    CollectionContainer = 
    {
        new NamedOnOffButton(),
        new BorderedButton()
        {
            MouseInput = 
            { 
                LeftClickedCallback = (obj, args) => 
                { 
                    MyAPIGateway.Utilities.ShowMessage("ScrollBox Example", "Button clicked"); 
                }
            }
        },
        new ColorPickerRGB() { Name = "RGBA Color" },
        new Label()
        { 
            Text = "Text Field Label", 
            AutoResize = false,
            Height = 20f
        },
        new TextField() 
        {
            UpdateValueCallback = (obj, args) => 
            { 
                var field = obj as TextField;
                MyAPIGateway.Utilities.ShowMessage("ScrollBox Example", $"TextField Updated: {field.Value}"); 
            }
        },
        new NamedCheckBox(),
        new ColorPickerHSV() { Name = "HSV Color" },
        new ListBox<string>()
        {
            { "Key 1", "Value 1" },
            { "Key 2", "Value 2" },
            { "Key 3", "Value 3" },
            { "Key 4", "Value 4" },
            { "Key 5", "Value 5" },
            { "Key 6", "Value 6" },
            { "Key 7", "Value 7" },
            { "Key 8", "Value 8" },
        }
    }
};
```
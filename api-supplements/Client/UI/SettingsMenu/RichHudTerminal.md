---
uid: RichHudFramework.UI.Client.RichHudTerminal
remarks: *content
---

This serves as the main entry point for adding settings, keybind customization, and information pages to the shared settings menu.

To add content, access the mod-specific root container via the <xref:RichHudFramework.UI.Client.RichHudTerminal.Root> property.

### UI Hierarchy
The structure of the terminal UI is dictated by the structure of the container heirarchy, starting with pages attached to `Root`. Controls are organized using the following container types:

* <xref:RichHudFramework.UI.Client.TerminalPageCategory>: (Optional) Groups related pages into a collapsible folder in the sidebar.
* <xref:RichHudFramework.UI.Client.ControlPage>: Represents an individual settings page that appears as an entry in the sidebar.
* <xref:RichHudFramework.UI.Client.ControlCategory>: A horizontally scrolling row within a `ControlPage`.
* <xref:RichHudFramework.UI.Client.ControlTile>: A vertical column within a `ControlCategory`.
* <xref:RichHudFramework.UI.Client.TerminalControlBase>: The individual interactive UI elements (buttons, sliders, checkboxes, etc.) placed inside `ControlTile` containers.

### Specialized Pages

In addition to standard controls, the framework provides specific page types for other needs:

* <xref:RichHudFramework.UI.Client.RebindPage>: A dedicated interface for managing custom keybindings via registered <xref:RichHudFramework.UI.IBindGroup>s.
* <xref:RichHudFramework.UI.Client.TextPage>: Displays a full page of read-only rich text, suitable for help manuals or changelogs.

### Example Heirarchy

```
RichHudTerminal.Root
├── ControlPage
│   ├── ControlCategory
│   │   ├── ControlTile
│   │   │   ├── TerminalSlider
│   │   │   └── TerminalCheckbox
│   │   └── ControlTile
│   │       ├── TerminalDropdown
│   │       ├── TerminalTextField
│   │       └── ...
│   └── ...
├── TerminalPageCategory
│   ├── TextPage
│   └── TextPage
├── RebindPage
└── ...
```

---
uid: RichHudFramework.UI.Client.RichHudTerminal
example: [*content]
---

The following example demonstrates how to define a menu structure using collection initializers. It adds a single `ControlPage` containing a `ControlCategory` and two controls (a slider and a checkbox) to the mod's root.

```csharp
float extSliderValue = 0.314159f;
bool extCheckboxValue = false;

// Make the root visible and interactable
RichHudTerminal.Root.Enabled = true;

// Access the mod root and add a new page
RichHudTerminal.Root.Add(new ControlPage
{
    Name = "Settings",
    CategoryContainer =
    {
        new ControlCategory
        {
            HeaderText = "Mod Features",
            TileContainer =
            {
                new ControlTile
                {
                    new TerminalSlider
                    {
                        Name = "Limit",
                        Min = 0f,
                        Max = 10f,
                        ToolTip = "This tooltip appears when mousing over the slider.",
                        // Initial value. The getter ensures the GUI stays synchronized 
                        // if the external variable changes elsewhere.
                        CustomValueGetter = () => extSliderValue,
                        ControlChangedHandler = (obj, args) =>
                        {
                            var slider = (TerminalSlider)obj;
                            extSliderValue = slider.Value;
                            // Slider value text is not automatic
                            slider.ValueText = $"{extSliderValue:G4} MyUnits";
                        }
                    },
                    new TerminalCheckbox
                    {
                        Name = "Feature Toggle",
                        CustomValueGetter = () => extCheckboxValue,
                        ControlChangedHandler = (obj, args) =>
                        {
                            var checkbox = (TerminalCheckbox)obj;
                            extCheckboxValue = checkbox.Value;
                        }
                    }
                }
            }
        }
    }
});
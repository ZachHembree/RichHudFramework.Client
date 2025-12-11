# Key UI Elements Glossary

This glossary provides a quick reference to the primary built-in visual and interactive UI elements available for building user interfaces available in the built-in library. For detailed documentation, see the API reference.

| Category | Name | Description | Use Case |
| :--- | :--- | :--- | :--- |
| **Basic Elements** | [TexturedBox](xref:RichHudFramework.UI.TexturedBox#RichHudFramework_UI_TexturedBox_examples) | A fundamental visual primitive that renders a colored, textured rectangle. | Creating backgrounds, custom dividers, or visual containers. |
| | [Label](xref:RichHudFramework.UI.Label#RichHudFramework_UI_Label_examples) | A text-only component dedicated to rendering formatted text ([RichText](xref:RichHudFramework.UI.RichText)) without a background. | Displaying overlays, floating text, or captions where a background is unnecessary. |
| | [LabelBox](xref:RichHudFramework.UI.LabelBox) | A composite element that layers text over a textured background. | Standard text displays requiring a visible background for contrast/readability. |
| **Buttons** | [BorderedButton](xref:RichHudFramework.UI.BorderedButton#RichHudFramework_UI_BorderedButton_examples) | A clickable button styled to mimic the native Space Engineers terminal interface. | Primary actions (e.g., "Save", "Apply") that must blend with the game's UI. |
| | [LabelBoxButton](xref:RichHudFramework.UI.LabelBoxButton#RichHudFramework_UI_LabelBoxButton_examples) | A generic clickable button featuring text on a background, including highlighting. | Custom button styles or multi-purpose interactive labels. |
| | [LabelButton](xref:RichHudFramework.UI.LabelButton#RichHudFramework_UI_LabelButton_examples) | A minimalist clickable text element that lacks a background texture. | Hyperlinks, navigation tabs, or low-profile actions. |
| **Input & Toggles** | [TextField](xref:RichHudFramework.UI.TextField) | A single-line input field featuring a background and border. | Capturing simple alphanumeric data (e.g., naming an entity or typing in a numeric value). |
| | [TextBox](xref:RichHudFramework.UI.TextBox) | A raw, multi-line capable text input supporting carets and highlighting, but without a background or scrollbars. | Custom input fields requiring distinct styling or embedding within complex containers. |
| | [NamedCheckBox](xref:RichHudFramework.UI.NamedCheckBox#RichHudFramework_UI_NamedCheckBox_examples) | A standard toggle control consisting of a checkbox and a descriptive label. | Binary (On/Off) settings where screen space is at a premium. |
| | [NamedOnOffButton](xref:RichHudFramework.UI.NamedOnOffButton#RichHudFramework_UI_NamedOnOffButton_examples) | A distinct boolean toggle featuring separate "On" and "Off" buttons. | High-visibility settings requiring clear state indication (SE Terminal style). |
| | [SliderBox](xref:RichHudFramework.UI.SliderBox#RichHudFramework_UI_SliderBox_examples) | A horizontal sliding control for selecting numeric values within a defined range. | Continuous adjustments like volume, sensitivity, or opacity. |
| | [ColorPickerHSV](xref:RichHudFramework.UI.ColorPickerHSV) | A color selection interface using Hue, Saturation, and Value sliders. | Intuitive color configuration for visual design tools. |
| | [ColorPickerRGB](xref:RichHudFramework.UI.ColorPickerRGB) | A color selection interface using Red, Green, and Blue sliders. | Precise color configuration requiring specific RGB values. |
| **Lists & Selection** | [Dropdown](xref:RichHudFramework.UI.Dropdown`3#RichHudFramework_UI_Dropdown_3_examples) | A collapsable list box styled to mimic the SE terminal dropdown menu. | Selecting one item from a list while conserving screen space. |
| | [ListBox](xref:RichHudFramework.UI.ListBox`3#RichHudFramework_UI_ListBox_3_examples) | A scrollable list of data-associated text entries. | Displaying and allowing selection from a scrolling list of options. |
| | [TreeList](xref:RichHudFramework.UI.TreeList`3#RichHudFramework_UI_TreeList_3_examples) | An indented, collapsable list for displaying hierarchical data. | Presenting structured, expandable data (like a configuration tree). |
| | [TreeBox](xref:RichHudFramework.UI.TreeBox`2#RichHudFramework_UI_TreeBox_2_examples) | Generalized `TreeList` that allows arbitrary mixed-type entries. | Advanced hierarchies requiring mixed leaf nodes and folder structures. |
| | [RadialSelectionBox](xref:RichHudFramework.UI.RadialSelectionBox`2) | A selection wheel (pie-menu style) displaying entries in a circular pattern. | Quick, gesture-based selection in a circular, compact menu. |
| **Containers & Layout** | [WindowBase](xref:RichHudFramework.UI.WindowBase) | A foundation for creating windows, including header, body, and border framing. | Building draggable, resizable dialogs or main interface panels. |
| | [HudChain](xref:RichHudFramework.UI.HudChain`2#RichHudFramework_UI_HudChain_2_examples) | A layout container that automatically arranges children in a horizontal or vertical stack. | Aligning UI components dynamically without manual coordinate management. |
| | [ScrollBox](xref:RichHudFramework.UI.ScrollBox`2#RichHudFramework_UI_ScrollBox_2_examples) | A container that clips overflowing content and provides scrollbars for navigation. | Presenting long, scrolling content within fixed screen real estate. |

>[!TIP]
>Many of the interactive UI elements above (Toggles, Sliders, and Lists) implement the [IValueControl<T>](xref:RichHudFramework.UI.IValueControl`1) interface. This provides a unified way to interact with controls that hold a changing value using events.

## Input Handling Elements

These are non-visual elements that provide core functionality for key binds and mouse interaction with your visible UI Elements.

| Name | Description | Use Case |
| :--- | :--- | :--- |
|[BindInputElement](xref:RichHudFramework.UI.BindInputElement#RichHudFramework_UI_BindInputElement_examples) | A utility for mapping custom keybinds or control combinations to UI elements. | Defining custom keyboard shortcuts or gamepad button presses that trigger specific actions on a focused UI element. |
| [MouseInputElement](xref:RichHudFramework.UI.MouseInputElement) | The foundational component for processing cursor interactions (clicks, hover states, tooltips). | Adding mouse interactions (clickable/hoverable) to UI elements that lack them and enabling tooltips. |
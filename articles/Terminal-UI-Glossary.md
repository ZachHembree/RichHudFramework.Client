# Terminal UI Glossary

This glossary provides a quick reference to controls and pages available in the [Rich HUD Terminal](xref:RichHudFramework.UI.Client.RichHudTerminal#RichHudFramework_UI_Client_RichHudTerminal_remarks). These types are unique to the terminal, and are designed specifically to match Space Engineer's UI style.

| Category | Name | Description | Use Case |
| :--- | :--- | :--- | :--- |
| **Page Container** | [TerminalPageCategory](xref:RichHudFramework.UI.Client.TerminalPageCategory) | A collapsable group of pages that can be added to a mod's control root. Functions as a folder in the sidebar navigation. | Grouping related pages into a collapsible folder in the sidebar navigation. |
| **Page** | [ControlPage](xref:RichHudFramework.UI.Client.ControlPage) | A page that organizes settings into vertically scrolling `ControlCategory`s. | Organizing general settings and options using a combination of control rows and tiles. |
| | [RebindPage](xref:RichHudFramework.UI.Client.RebindPage) | A terminal page designed specifically for managing key bindings for `IBindGroup`s in a scrolling list. It features a pop-up dialog for changing key combinations. | Allowing users to view and change key bindings for specific groups. |
| | [TextPage](xref:RichHudFramework.UI.Client.TextPage) | A scrolling page of wrapped, read-only text. | Displaying help manuals, changelogs, or information displays. |
| **Container** | [ControlCategory](xref:RichHudFramework.UI.Client.ControlCategory) | A horizontally scrolling row of `ControlTile`s within a `ControlPage`. | Visually grouping related controls/tiles into a single horizontal segment within a settings page. |
| | [ControlTile](xref:RichHudFramework.UI.Client.ControlTile) | A vertical layout column within a `ControlCategory`. | Grouping a small, vertical set of controls (generally no more than 3). |
| **Control** | [TerminalLabel](xref:RichHudFramework.UI.Client.TerminalLabel) | A static text label for a `ControlTile`. | Labeling a group of controls on a tile. |
| | [TerminalTextField](xref:RichHudFramework.UI.Client.TerminalTextField) | A single-line input field featuring a background and border. | Capturing simple alphanumeric data (e.g., naming an entity or typing in a numeric value). |
| | [TerminalButton](xref:RichHudFramework.UI.Client.TerminalButton) | A clickable button for triggering an action. | Primary actions (e.g., "Save", "Apply"). |
| | [TerminalCheckbox](xref:RichHudFramework.UI.Client.TerminalCheckbox) | 	A standard toggle control consisting of a checkbox and a descriptive label. | Binary (On/Off) settings where screen space is at a premium. |
| | [TerminalOnOffButton](xref:RichHudFramework.UI.Client.TerminalOnOffButton) |  A distinct boolean toggle featuring separate "On" and "Off" buttons. | High-visibility settings requiring clear state indication. |
| | [TerminalSlider](xref:RichHudFramework.UI.Client.TerminalSlider) | 	A horizontal sliding control for selecting numeric values within a defined range. | Continuous adjustments like volume, sensitivity, or opacity. |
| | [TerminalColorPicker](xref:RichHudFramework.UI.Client.TerminalColorPicker) | An RGB color picker using three sliders (Red, Green, Blue). | Allowing users to manually configure colors. |
| | [TerminalDragBox](xref:RichHudFramework.UI.Client.TerminalDragBox) | A control allowing the user to visually select a 2D screen position (Vector2). It spawns a temporary draggable window when interacted with. | User configurable HUD layout. |
| | [TerminalDropdown](xref:RichHudFramework.UI.Client.TerminalDropdown`1) | A collapsing dropdown list with labels paired with data. | Compactly presenting a fixed list of choices in a confined space. |
| | [TerminalList](xref:RichHudFramework.UI.Client.TerminalList`1) | A non-collapsing, fixed-height list box with labels paired with data. | Presenting a fixed non-collapsing list of choices displayed in an open box. |
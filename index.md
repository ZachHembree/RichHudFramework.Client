# Overview
The **Rich HUD Framework** is a modding library enabling the creation of fully custom, retained-mode Graphical User Interfaces (GUI) within Space Engineers C# mods. RHF provides the essential architecture for building **scalable, layered, and interactive** user interfaces within the constraints of the Space Engineers modding API. It does this by offering:

  * **Declarative UI:** A library of standard controls (buttons, dropdowns, sliders) that minimizes boilerplate code and facilitates a declarative approach to UI construction.
  * **Extensive Customization:** Flexibility for imperative design and deep customization via subclassing.
  * **API Enhancements:** It fills critical gaps in the modding API by including:
      * A system for managing **key binds**.
      * A custom, flexible **text renderer**.

## Get Started

Ready to jump into the code?

  * **Installation:** See the article on [Installation and Mod Integration](~/articles/Installation-and-Mod-Integration.md).
  * **Example:** Explore a practical application in the [Text Editor Example](https://github.com/ZachHembree/TextEditorExample) repository.

> For general Space Engineers modding reference, consult the official [Space Engineers Wiki](https://spaceengineers.wiki.gg/wiki/Modding/Reference/ModScripting).

## The Node Graph
At the heart of the framework is a **retained-mode UI tree**. Unlike immediate-mode GUI systems where elements are defined and drawn every frame, elements in this framework are persistent objects in memory, organized into a hierarchy of parents and children.

**Core Principles**

  * **Hierarchy:** Every UI element must be attached to a **parent node** to be rendered or processed. The update cycle propagates from the root down to leaf nodes.
  * **Retained-Mode:** Elements are objects that persist across frames, which allows the framework to efficiently manage state and updates.

**Coordinate Spaces and Scaling**

  * **Coordinate Spaces:** The framework supports both **screen-space** rendering (2D overlays) and **world-space** rendering (3D displays attached to in-game entities). This is managed through [HUD Spaces](~/articles/HUD-Spaces.md), which allow any subtree to be attached to a custom coordinate space driven by an arbitrary world matrix.
  * **Resolution Independence:** The optional <xref:RichHudFramework.UI.Client.HudMain.HighDpiRoot> node scales UI elements to maintain consistent sizing for screen-space UI across different resolutions (e.g., 1080p vs. 4K).

> [HudMain](xref:RichHudFramework.UI.Client.HudMain#RichHudFramework_UI_Client_HudMain_remarks) is the entry point for the client-side UI system. From here, you access the root nodes, screen metrics, and the shared cursor.

### Hierarchy Example

```c
Root // Screen-sized container, no DPI scaling
├── HighDpiRoot // Automatically handles DPI and resolution scaling
│   └── HudChain // Layout manager (e.g., vertical stack)
│       └── { Label, Button, TextField } // Child elements
└── TexturedBox // Background
    └── Label // Text draws over TexturedBox
```

### UI Elements
<xref:RichHudFramework.UI.HudElementBase#RichHudFramework_UI_HudElementBase_remarks> is the practical starting point for modded UI. Each element has explicit size and padding, supports parent-relative or offset positioning, offers built-in alignment modes (<xref:RichHudFramework.UI.HudElementBase.ParentAlignment> and <xref:RichHudFramework.UI.HudElementBase.DimAlignment>), and exposes update hooks required for deep customization.

- Complex layouts can be built declaratively using chains, collections, and alignment properties – often without writing any manual positioning code. Nested horizontal/vertical [HudChain](xref:RichHudFramework.UI.HudChain`2#RichHudFramework_UI_HudChain_2_remarks) instances serve a similar role to CSS Flexbox or WPF StackPanel, while [ScrollBox](xref:RichHudFramework.UI.ScrollBox`2#RichHudFramework_UI_ScrollBox_2_remarks) adds scrolling capability.

- <xref:RichHudFramework.UI.TexturedBox> and <xref:RichHudFramework.UI.Label> are the simplest concrete types in the framework, providing basic textured quad and text rendering, respectively. These can serve as a good starting point when learning how to initialize visible UI.

>[!NOTE]
>While the framework includes several pre-built elements, more sophisticated custom UIs will often require extending `HudElementBase` or other descendants directly to implement custom layout and input logic. See the API reference for `HudElementBase` for details.

## Input & Key Binds
The <xref:RichHudFramework.UI.Client.BindManager#RichHudFramework_UI_Client_BindManager_remarks> serves as the central hub for handling user input. It facilitates the creation of custom key binds and control groups that can be serialized for persistence.

* **Grouped Binds:** Inputs are organized into groups to prevent conflicts.
* **Aliasing:** Each bind supports primary, secondary, and tertiary key combinations (aliases).
* **Blacklisting:** The manager handles the **suppression** of standard game inputs (such as camera rotation or shooting) while UI elements are interactive, preventing accidental interactions with the game world.

## The Settings Menu
To streamline mod configuration and promote a more consistent user experience, the framework provides a shared settings menu (<xref:RichHudFramework.UI.Client.RichHudTerminal#RichHudFramework_UI_Client_RichHudTerminal_remarks>). Rather than rely on chat commands, manually-edited config files, or a bespoke GUI, you can register controls within this terminal and tie it into your own configuration system with simple event callbacks.

Mods contribute pages and controls via a simple container hierarchy:

- Control pages with sliders, checkboxes, dropdowns, etc.
- Dedicated ready-made rebind pages.
- Read-only rich-text pages for help/changelog content.

## Text Renderer
The framework includes a custom bitmap text renderer (exposed via <xref:RichHudFramework.UI.Rendering.ITextBoard#RichHudFramework_UI_Rendering_ITextBoard_remarks>), which supports custom fonts and is suitable for everything from a one-line plain text field to multi-line wrapped rich text.

* <xref:RichHudFramework.UI.Label>: The standard, non-interactive UI element used for displaying text. It serves as the main entry point for the text renderer for simple text fields.
* <xref:RichHudFramework.UI.RichText>: This class allows for the construction of formatted text strings where style, color, and size can vary per character.
* The <xref:RichHudFramework.UI.Rendering.Client.FontManager> allows mods to register and use custom texture-based fonts at runtime. See the article on [Custom Fonts](~/articles/Custom-Fonts.md) for details.

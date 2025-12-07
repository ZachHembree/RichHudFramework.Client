# Overview
The **Rich HUD Framework** (RHF) is a library for creating custom, retained-mode Graphical User Interfaces (GUI) within Space Engineers mods.

This framework provides the foundational architecture for building scalable, layered, and interactive user interfaces within the constraints of the modding API. While it includes a not-insubstantial library of standard controls (buttons, dropdowns, sliders), its primary strength lies in its node-based management system, which handles the complexities of input routing, depth sorting, and resolution scaling.

If you want to skip the prologue and jump into the code, see the article on [Installation and Mod Integration](~/articles/Installation-and-Mod-Integration.md) and the [Text Editor Example](https://github.com/ZachHembree/TextEditorExample).

> See the official [Space Engineers Wiki](https://spaceengineers.wiki.gg/wiki/Modding/Reference/ModScripting) for general modding reference.

## The Node Graph
At the heart of the framework is a retained-mode UI tree. Unlike immediate-mode GUI systems where elements are defined and drawn every frame, elements in this framework are objects that persist in memory. They are organized into a hierarchy of parents and children.

* **Hierarchy:** Every UI element must be attached to a parent node to be rendered or processed. The update cycle propagates from the root down to leaf nodes.
* **Coordinate Spaces:** The framework supports screen-space rendering (2D overlays) and world-space rendering (3D displays attached to in-game entities) through the use of [HUD Spaces](~/articles/HUD-Spaces.md), which allow any subtree to be attached to a custom coordinate space that is driven by an arbitrary world matrix.
* **Resolution Independence:** The framework provides specific root nodes that automatically scale UI elements to maintain consistent sizing across different screen resolutions (e.g., 1080p vs. 4K).

The entry point for the client-side UI system is [HudMain](xref:RichHudFramework.UI.Client.HudMain#RichHudFramework_UI_Client_HudMain_remarks). From here, you access the root nodes, screen metrics, and the shared cursor.

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
* **Blacklisting:** The manager handles the suppression of standard game inputs (such as camera rotation or shooting) while UI elements are interactive, preventing accidental interactions with the game world.

## The Settings Menu
To streamline mod configuration and promote a more consistent user experience, the framework provides a shared settings menu (<xref:RichHudFramework.UI.Client.RichHudTerminal#RichHudFramework_UI_Client_RichHudTerminal_remarks>). Rather than rely on chat commands, manually-edited config files, or a bespoke GUI, you can register controls within this terminal and tie it into your own configuration system with simple event callbacks.

Mods contribute pages and controls via a simple container hierarchy:

- Control pages with sliders, checkboxes, dropdowns, etc.
- Dedicated ready-made rebind pages.
- Read-only rich-text pages for help/changelog content.

## Text & Fonts
The framework includes a custom bitmap text renderer, suitable for everything from a one-line plain text field to multi-line wrapped rich text.

* <xref:RichHudFramework.UI.RichText>: This class allows for the construction of formatted text strings where style, color, and size can vary per character.
* <xref:RichHudFramework.UI.Rendering.ITextBoard#RichHudFramework_UI_Rendering_ITextBoard_remarks>: The interface for low-level rendering logic and formatting.
* **Custom Fonts:** The <xref:RichHudFramework.UI.Rendering.Client.FontManager> allows mods to register and use custom texture-based fonts at runtime. See the article on [Custom Fonts](~/articles/Custom-Fonts.md) for details.

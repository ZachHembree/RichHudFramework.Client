---
uid: RichHudFramework.UI.HudParentBase
remarks: *content
---
`HudParentBase` serves as the fundamental base class for all components capable of serving as parent nodes in the UI tree. It defines the core lifecycle hooks for the framework's update loop, including `Measure`, `Layout`, `Draw`, and input handling.

Unlike its derived types, this class represents a root-level object; types deriving strictly from `HudParentBase` cannot be parented to other elements. Consequently, it is primarily used as the foundation for the framework's global root nodes, such as <xref:RichHudFramework.UI.Client.HudMain.Root> and <xref:RichHudFramework.UI.Client.HudMain.HighDpiRoot>, which serve as the anchors for the entire UI node graph.

>[!NOTE]
>It is generally not recommended to extend this type directly for custom controls. In standard usage, <xref:RichHudFramework.UI.HudElementBase> should be used as the base type for creating custom UI elements, as it includes the necessary logic for sizing, positioning, and parenting.

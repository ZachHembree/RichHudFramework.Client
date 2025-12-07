---
uid: RichHudFramework.UI.IReadOnlyHudSpaceNode
remarks: *content
---
The coordinate system for all UI elements within the framework is defined by the <xref:RichHudFramework.UI.HudParentBase.HudSpace> property, which uses this interface. Fundamentally, every UI element is rendered on the **X/Y (Right/Up)** plane of the matrix transform defined by <xref:RichHudFramework.UI.IReadOnlyHudSpaceNode.PlaneToWorld>.

For a comprehensive overview of the theoretical concepts regarding coordinate systems in this framework, refer to the article on [HUD Spaces](~/articles/HUD-Spaces.md).

### Implementations

Several classes implement this interface to provide specific coordinate systems:

* <xref:RichHudFramework.UI.Client.HudMain.Root> and <xref:RichHudFramework.UI.Client.HudMain.HighDpiRoot>:
    Managed by <xref:RichHudFramework.UI.Client.HudMain>, these are specialized implementations optimized for standard screen-space emulation.

* <xref:RichHudFramework.UI.CustomSpaceNode>:
    This node offers maximum flexibility by allowing the world matrix to be defined via a **delegate callback**. It is particularly useful for attaching UI elements to dynamic entities (see: [IMyEntity.WorldMatrix](https://keensoftwarehouse.github.io/SpaceEngineersModAPI/api/VRage.Game.ModAPI.Ingame.IMyEntity.html#VRage_Game_ModAPI_Ingame_IMyEntity_WorldMatrix)) where the transform matrix changes every frame.

* <xref:RichHudFramework.UI.CamSpaceNode>:
    Generates its coordinate system based directly on the **camera matrix**. It allows for custom rotation, translation offsets, and optional screen-space scaling, enabling the creation of HUDs that follow the player's view but with specific modifications.

* <xref:RichHudFramework.UI.ScaledSpaceNode>:
    Modifies an existing HUD space by applying a **uniform scalar** to the coordinate system. This is primarily used to implement uniform scaling for all child UI elements attached to that node.

* <xref:RichHudFramework.UI.HudSpaceNodeBase>:
    The abstract base class for most concrete implementations of this interface. It handles the necessary boilerplate for API integration and manages **cursor position projection**. This ensures that every node within the space's subtree receives accurate cursor coordinates relative to its local coordinate system.
---
uid: RichHudFramework.UI.HudNodeBase
remarks: *content
---
`HudNodeBase` extends the functionality of <xref:RichHudFramework.UI.HudParentBase> by adding the capability to register as a child of another node. This class represents the minimal implementation required for an object to exist within the UI hierarchy, providing essential properties such as `Parent`, `Registered`, and `ZOffset`, alongside methods for registration and unregistration.

This class is principally designed for logic-only UI components that require access to the update loop but do not need definite screen dimensions, such as <xref:RichHudFramework.UI.BindInputElement>. It also serves as the base for more specialized nodes, like implementations of <xref:RichHudFramework.UI.IReadOnlyHudSpaceNode>, and the common <xref:RichHudFramework.UI.HudElementBase>.

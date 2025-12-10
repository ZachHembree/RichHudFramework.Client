---
uid: RichHudFramework.UI.HudCollection`2
remarks: *content
---

The `HudCollection` class serves as the fundamental storage mechanism for managing ordered lists of HUD elements. By pairing child elements with customizable wrapper objects (implementing <xref:RichHudFramework.UI.IHudNodeContainer`1>), the collection allows developers to associate metadata or extend functionality without modifying the underlying element.

This infrastructure is the foundation for several key layout and control types:

  * <xref:RichHudFramework.UI.HudChain`2>: Uses the collection to implement linear layout logic, organizing elements into vertical or horizontal stacks. Unlike standard parenting, the chain order determines the visual sequence. Chain-specific wrappers (implementing <xref:RichHudFramework.UI.IChainElementContainer`1>) introduce layout properties such as <xref:RichHudFramework.UI.IChainElementContainer`1.AlignAxisScale>. These properties work in conjunction with the <xref:RichHudFramework.UI.HudChain`2.SizingMode> to control the dynamic sizing and alignment of members within the stack.
  * <xref:RichHudFramework.UI.ScrollBox`2>: Builds upon the `HudChain` architecture to create scrollable regions. It utilizes a specialized container interface that adds an <xref:RichHudFramework.UI.IScrollBoxEntry`1.Enabled> property. This allows specific entries to be dynamically hidden or shown within the scrolling list without creating layout gaps or removing them from the underlying collection.
  * <xref:RichHudFramework.UI.SelectionBoxBase`3>: Extends the collection functionality to support complex interaction patterns and data binding. By implementing interfaces such as <xref:RichHudFramework.UI.IEntryBox`2>, these types associate UI elements with data tuples (via [IScrollBoxEntryTuple](xref:RichHudFramework.UI.IScrollBoxEntryTuple`2) and [ISelectionBoxEntryTuple](xref:RichHudFramework.UI.ISelectionBoxEntryTuple`2)) and often integrate object pooling. This flexibility supports the creation of mixed-type lists, where distinct entry types—such as simple labels and folder groups—can coexist within the same structure.

A key advantage of this modular design is the decoupling of storage and layout. Because the container logic is generic, a specific control can often be configured to use either a static `HudChain` or a scrolling `ScrollBox` by simply changing the generic type argument, while retaining the same entry management logic.

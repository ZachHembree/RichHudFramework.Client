---
uid: RichHudFramework.UI.SelectionBoxBase`3
remarks: *content
---
`SelectionBoxBase` is the foundational abstract class for all selection-style list controls. It combines **standardized input handling** with a <xref:RichHudFramework.UI.ListInputElement`2> component to deliver core functionality. This includes features such as **highlighting**, **selection** management, and **entry hiding**.

The list's layout and scrolling behavior are governed by the generic **TChain** parameter, which defines the underlying container. This container is typically a <xref:RichHudFramework.UI.HudChain`2> for **non-scrolling** lists or a <xref:RichHudFramework.UI.ScrollBox`2> for **scrollable** lists. In both cases, the container manages the collection of elements through a <xref:RichHudFramework.UI.HudCollection`2>.

### Concrete Implementations

To simplify usage, each concrete implementation of `SelectionBoxBase` provides aliases for its default generic parameters (`TElementContainer` and `TElement`).

The primary specialization, <xref:RichHudFramework.UI.SelectionBox`4>, forms the basis for selection boxes that use **reusable and uniform pooled entries**. It offers two main variants based on the container type:

* <xref:RichHudFramework.UI.ChainSelectionBox`3>: The **non-scrolling** variant.
* <xref:RichHudFramework.UI.ScrollSelectionBox`3>: The **scrolling** variant.

Additional derived types include:

* <xref:RichHudFramework.UI.ListBox`3>: A non-collapsing, scrolling list derived from `ScrollSelectionBox`.
* <xref:RichHudFramework.UI.Dropdown`3>: A collapsible scrolling list that incorporates `ListBox` internally.
* <xref:RichHudFramework.UI.TreeBox`2>: A non-scrolling, indented, collapsible list suited for heterogeneous, non-pooled entries.
* <xref:RichHudFramework.UI.TreeList`3>: A non-scrolling, indented, collapsible list optimized for uniform, pooled entries.
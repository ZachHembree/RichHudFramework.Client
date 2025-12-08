---
uid: RichHudFramework.UI.Client.HudMain
remarks: *content
---
`HudMain` is the entry point for the framework's tree management and update system. It connects individual mod UI subtrees to the Rich HUD Master module, facilitating the creation of a unified and efficient node graph.

All UI elements created via the framework must ultimately be attached to one of the provided root nodes to be rendered and processed. This architecture allows for stable, load-order independent layering and seamless depth sorting between different mods.

* <xref:RichHudFramework.UI.Client.HudMain.Root>: This node represents the standard pixel-space root. Elements attached here map 1:1 with screen pixels. This is useful for manual scaling or fixed-size UI.
* <xref:RichHudFramework.UI.Client.HudMain.HighDpiRoot>: This node is designed for resolution independence, and is the **recommended** default for new UI. It automatically applies a scaling factor (<xref:RichHudFramework.UI.Client.HudMain.ResScale>) to its children, ensuring that UI elements maintain a consistent apparent size on high-DPI displays (such as 1440p or 4K monitors) without requiring manual layout adjustments.

### Update Cycle

The framework manages the update logic for the entire node graph to ensure synchronization. Update passes are handled internally, propagating from the root down to leaf nodes. For a detailed explanation of the update cycle (including `Measure`, `Layout`, and `Draw` passes), refer to the documentation for <xref:RichHudFramework.UI.HudElementBase#RichHudFramework_UI_HudElementBase_remarks>.

### Input Management

The framework creates a unified input environment to handle mouse and keyboard interactions across multiple mods.

* **Shared Cursor:** Access to the global cursor is provided via the <xref:RichHudFramework.UI.Client.HudMain.Cursor> property. This interface allows for position querying, tooltip registration, and capture logic. However, direct interaction with the cursor interface is rarely necessary for standard UI components; the framework automatically handles interaction via the <xref:RichHudFramework.UI.IMouseInput> interface and the <xref:RichHudFramework.UI.HudParentBase.HudSpace>.
* **Cursor State:** The <xref:RichHudFramework.UI.Client.HudMain.EnableCursor> and <xref:RichHudFramework.UI.IMouseInput.RequestCursor> properties are used to signal the framework that the mod requests the cursor to be visible.
* **Input Mode:** The <xref:RichHudFramework.UI.Client.HudMain.InputMode> property reflects the current state of the input system, indicating whether the cursor is visible or if text input is active.

> [!NOTE]
> `EnableCursor` does not indicate when the cursor has been enabled through other means, such as by other mods or incidentally by built-in Space Engineers UI. Always check `InputMode` to determine the actual input state.

### Coordinate Systems and Metrics

While `HudMain` provides the 2D screen-space roots, the framework allows the definition of custom coordinate systems via **HUD Spaces**. These allow UI subtrees to be mapped to 3D world coordinates, such as cockpit displays. Refer to the article on [HUD Spaces](~/articles/HUD-Spaces.md) for details.

To assist with layout and rendering, `HudMain` exposes several real-time screen metrics:
* **Screen Dimensions:** <xref:RichHudFramework.UI.Client.HudMain.ScreenWidth>, <xref:RichHudFramework.UI.Client.HudMain.ScreenHeight>, and <xref:RichHudFramework.UI.Client.HudMain.AspectRatio>.
* **Scaling:** <xref:RichHudFramework.UI.Client.HudMain.ResScale> provides the scaling factor used by the `HighDpiRoot`, normalized to a 1080p baseline.
* **Projection:** <xref:RichHudFramework.UI.Client.HudMain.PixelToWorld> provides the matrix required to convert 2D screen-space coordinates into 3D world-space positions for rendering.

### Clipboard Integration

The <xref:RichHudFramework.UI.Client.HudMain.ClipBoard> property provides a shared, read/write clipboard specific to the framework. This clipboard supports <xref:RichHudFramework.UI.RichText>, preserving formatting data (colors and fonts) when copying text between framework controls, such as <xref:RichHudFramework.UI.TextField> and <xref:RichHudFramework.UI.TextBox>.

> [!NOTE]
> This is separate from the write-only system clipboard API (<xref:VRage.Utils.MyClipboardHelper>).
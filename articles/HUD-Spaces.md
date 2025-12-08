# HUD Spaces

This framework renders all UI elements with textured quads positioned in **world space**. Each element belongs to a **HUD Space Node** that defines a 2D plane embedded within the 3D world. Child elements are drawn onto this plane using the node's **Plane-To-World (PTW)** matrix.

By default, UI elements inherit the **Pixel-To-World** transform provided by <xref:RichHudFramework.UI.Client.HudMain>. This transform emulates screen-space rendering: units are in pixels, the origin (0, 0) is at the screen center, and elements appear fixed relative to the viewport. Although the underlying geometry is still world-space billboards (required by the Space Engineers rendering API), this default behavior is functionally equivalent to a 2D screen overlay and is referred to throughout the documentation as “screen space” for simplicity.

### Space Node Types

The framework currently provides three concrete HUD space node implementations:

| Type                    | Purpose                                                                                                   | Recommended Use Case                                                                 |
|-------------------------|-----------------------------------------------------------------------------------------------------------|---------------------------------------------------------------------------------------------|
| <xref:RichHudFramework.UI.ScaledSpaceNode> | Applies a uniform scale factor to the parent space's XY plane. | Simple resizing of screen-space UI (e.g., global UI scaling, high-DPI adjustments)        |
| <xref:RichHudFramework.UI.CustomSpaceNode> | Allows complete control over the PTW matrix via a user-supplied delegate callback every frame | Attaching UI to dynamic world entities, fully custom coordinate systems             |
| <xref:RichHudFramework.UI.CamSpaceNode> | Constructs the PTW matrix from the player camera matrix with optional rotation, translation, and scaling | View-aligned “floating” HUDs or world-space panels that track the camera |

If matrix mathematics or coordinate-space transformations are unfamiliar, the Transformations section of [LearnOpenGL](https://learnopengl.com/Getting-started/Transformations) has an excellent primer.

### Core Functions

Every HUD space node (via its base class <xref:RichHudFramework.UI.HudSpaceNodeBase>) performs three critical tasks each frame:

1. **PTW Matrix Calculation** – Supplies the transformation that positions and orients its plane in world space.
2. **Cursor Projection** – Computes the exact world-space point where the mouse cursor intersects the plane, then transforms that point back into the node's local 2D coordinates. This ensures accurate mouse-over detection and input handling regardless of the node's orientation or distance from the camera.
3. **Depth Sorting Reference** – Provides a representative 3D position (usually the plane origin) used for painter's-algorithm sorting of semi-transparent UI elements.

### Use Cases
HUD Spaces can be divided into two categories of behavior: screen-space and world-space.

#### Screen-Space Emulation

<xref:RichHudFramework.UI.Client.HudMain.Root> and <xref:RichHudFramework.UI.Client.HudMain.HighDpiRoot> are pre-configured nodes that place their plane on the camera's near clip plane with pixel units. This is how the framework achieves the illusion of a true 2D overlay despite the API only exposing 3D billboards (see: <xref:VRage.Game.MyTransparentGeometry>).

#### Diegetic / World-Space UI

By parenting UI elements to a `CustomSpaceNode` whose matrix is derived from a cockpit block, door, or any <xref:VRage.Game.ModAPI.Ingame.IMyEntity>, developers can create interfaces that naturally move, rotate, and scale with objects in the game world (e.g., control panels, status readouts on machinery, holographic menus).

### Depth Sorting

All UI elements are rendered as semi-transparent textured quads. Correct blending order is essential to avoid visual artifacts. The framework sorts entire HUD spaces (not individual elements) by the distance from the camera to each space's origin. Spaces farther away are drawn first; closer spaces are drawn last. This per-space painter's algorithm ensures consistent and predictable transparency behavior even when multiple overlapping world-space HUDs are present.

### Live Demonstration
A demo of `CamSpaceNode` is available in the Rich HUD Terminal. Enable debugging with the chat command `/rhd toggleDebug` and open the Demo page to experiment with real-time scaling, rotation, and offset parameters.
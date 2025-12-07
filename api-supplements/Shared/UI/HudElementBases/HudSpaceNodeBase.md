---
uid: RichHudFramework.UI.HudSpaceNodeBase
remarks: *content
---
`HudSpaceNodeBase` is the abstract base class for all HUD nodes that define a custom coordinate system, by replacing the default screen-space `Pixel-to-World` transformation with an arbitrary world matrix.

This class provides a default implementation of the <xref:RichHudFramework.UI.IReadOnlyHudSpaceNode> interface, handling the complex boilerplate required for 3D interaction. Its features include:

* **Cursor Projection:** Automatically projects the screen-space cursor onto the node's custom plane to calculate accurate local cursor coordinates.
* **Visibility Checks:** Includes logic for determining if the node is in front of the camera and facing towards it (culling back-facing elements).
* **API Integration:** Provides default callbacks for the cursor system to query the node's current HUD space properties.

Common derived implementations include <xref:RichHudFramework.UI.CamSpaceNode> for camera-relative offsets and <xref:RichHudFramework.UI.CustomSpaceNode> for user-defined world matrices.
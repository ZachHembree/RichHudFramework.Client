---
uid: RichHudFramework.UI.TexturedBox
remarks: *content
---
This component serves as a minimal, non-interactive element for drawing textured quads based on <xref:RichHudFramework.UI.Rendering.Material>s.

Normally, the element utilizes the <xref:RichHudFramework.UI.Rendering.Material.Default> material, creating a blank, tintable box. The visual behavior can be further customized using the <xref:RichHudFramework.UI.TexturedBox.MatAlignment> property, which determines how the texture is scaled or cropped to fit the element's bounds (e.g., stretching to fit or preserving aspect ratio).

For details on custom material creation, see the [Custom Textures](~/articles/Custom-Textures.md) article.

---
uid: RichHudFramework.UI.TexturedBox
example: [*content]
---
The following example demonstrates the creation of a `TexturedBox` placed in the center of the screen using a built-in nebula texture from the Space Engineers skybox. It also places a second, blank `TexturedBox` on top of the first one.

```csharp
// Material handle to a texture that is 256x256
public static readonly Material TestTex = new Material("RHFNebulaTest", new Vector2(256));

var nebulaBox = new TexturedBox(HudMain.HighDpiRoot)
{
    Material = TestTex,
    // This mode doesn't allow cropping or warping, making 
    // this box effectively 720x720pts
    MatAlignment = MaterialAlignment.FitAuto,
    Size = new Vector2(1280, 720)
};

var blankBox = new TexturedBox(nebulaBox)
{
    ParentAlignment = ParentAlignments.InnerBottom,
    Height = 50f,
    DimAlignment = DimAlignments.Width
};
```

This is the transparent material definition added to the `.sbc` file for the custom material used above.

```xml
<TransparentMaterial>
    <Id>
        <TypeId>TransparentMaterialDefinition</TypeId>
        <SubtypeId>RHFNebulaTest</SubtypeId>
    </Id>
    <AlphaMistingEnable>false</AlphaMistingEnable>
    <Texture>Textures\BackgroundCube\Prerender\Crab_Nebula.dds</Texture>
    <Reflectivity>0</Reflectivity>
</TransparentMaterial>
```

# Custom Textures

The framework renders UI elements with the [MyTransparentGeometry](https://keensoftwarehouse.github.io/SpaceEngineersModAPI/api/VRage.Game.MyTransparentGeometry.html) API. This means custom textures follow the same workflow as other mods that use [Transparent Materials](https://spaceengineers.wiki.gg/wiki/Modding/Reference/Materials#Transparent_Materials): a `.dds` texture paired with an `.sbc` definition.

Textures are applied to UI elements via a <xref:RichHudFramework.UI.Rendering.Material> handle, which is initialized using the `SubtypeId` for that texture defined in your `.sbc` file. For a usage example, see the [TexturedBox](xref:RichHudFramework.UI.TexturedBox#RichHudFramework_UI_TexturedBox_examples) API reference page.

## Texture Conversion

Textures must be converted to **DDS format** using **BC7_UNORM** compression. Other block compression (BC) formats may work, but BC7 is a good default. Use Microsoft’s **texconv** utility, included with Space Engineers in `\SteamApps\common\SpaceEngineers\Tools\TexturePacking\Tools\texconv.exe`.

The following batch script examples will convert textures in the same directory to the given format using texconv (drag and drop), provided the batch file, executable and texture are in the same directory. If texconv outputs `.DDS` (uppercase), rename to lowercase `.dds`. The engine will complain otherwise.

**To DDS Batch**

```
texconv -y -f BC7_UNORM -pmalpha -if LINEAR %*
```

**To PNG Batch**

You should generally avoid converting from .dds, as it's a highly _lossy_ format. Originals should be kept in a lossless format, like  PNG, TIFF, PSD, etc.

```
texconv -y -ft PNG %*
```

>[!TIP]
> For advanced options, see the official [texconv documentation](https://github.com/microsoft/DirectXTex/wiki/texconv).

## Transparent Material Definitions

Create an `.sbc` for your textures based on the following template.

**Key Points**
- **SubtypeId** is the name you’ll reference in code when creating a `Material` (e.g., `new Material("MyUniqueSubtypeId", new Vector2(64))`).
- **Texture path** is relative to your mod’s root folder.

```xml
<?xml version='1.0'?>
<Definitions xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:xsd='http://www.w3.org/2001/XMLSchema'>
    <TransparentMaterials>
    <!-- Multiple TransparentMaterials can be defined in the same TransparentMaterials block -->
    <!-- Your first texture -->
        <TransparentMaterial>
            <Id>
                <TypeId>TransparentMaterialDefinition</TypeId>
                <!-- The name of your texture required by SE and referenced in RHF Material -->
                <SubtypeId>MyUniqueSubtypeId</SubtypeId>
            </Id>
            <!-- Path to your texture, inside your mod directory -->
            <Texture>MyTexturesFolder\MyTextureName.dds</Texture>
            <Reflectivity>0</Reflectivity>
        </TransparentMaterial>
        <!-- Your second texture...
        <TransparentMaterial>
        ...
        </TransparentMaterial> -->
    </TransparentMaterials>
</Definitions>
```

Save the file with a `.sbc` extension (e.g., `MyTextureDefinitions.sbc`) and place it anywhere inside your mod’s **Data** folder or its subfolders. The game automatically loads every `.sbc` it finds there.

### Example Folder Structure
```
MyMod/
├── Data/
│   └── MyTextureDefinitions.sbc
└── MyTexturesFolder/
    └── MyTextureName.dds
```
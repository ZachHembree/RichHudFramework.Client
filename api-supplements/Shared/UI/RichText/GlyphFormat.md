---
uid: RichHudFramework.UI.GlyphFormat
example: [*content]
---

```csharp
public static readonly GlyphFormat basicRed = new GlyphFormat(textSize: 1.0f, color: Color.Red);
public static readonly GlyphFormat fancyRed = basicRed.WithFont("AbhayaLibreMedium");
public static readonly GlyphFormat sansSerifHeader = new GlyphFormat(Color.LightBlue, TextAlignment.Center, 1.4f, "BitstreamVeraSans");
public static readonly GlyphFormat fluentPurple = GlyphFormat.White.
    WithColor(Color.Purple).
    WithSize(0.8f);
```
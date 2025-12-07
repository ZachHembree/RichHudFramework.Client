---
uid: RichHudFramework.UI.Rendering.ITextBuilder
remarks: *content
---
`ITextBuilder` is the text building and formatting interface for <xref:RichHudFramework.UI.Rendering.ITextBoard>. For details on usage, see that interface.

---
uid: RichHudFramework.UI.Rendering.ITextBoard
remarks: *content
---
The `ITextBoard` interface provides the fundamental rendering and layout logic for UI elements that display text. It acts as a bridge between the raw string manipulation provided by <xref:RichHudFramework.UI.Rendering.ITextBuilder> (which it inherits) and the visual presentation. This interface is the backing component for standard controls such as <xref:RichHudFramework.UI.Label> and <xref:RichHudFramework.UI.TextBox>.

#### Advanced Formatting and Layout

Unlike simple string or <xref:RichHudFramework.UI.RichText> properties, `ITextBoard` exposes granular control over text presentation. It manages line breaking via <xref:RichHudFramework.UI.Rendering.ITextBuilder.BuilderMode>, vertical alignment via <xref:RichHudFramework.UI.Rendering.ITextBoard.VertCenterText>, and dimensions via <xref:RichHudFramework.UI.Rendering.ITextBoard.AutoResize>. When `AutoResize` is disabled, the board clips content that exceeds its <xref:RichHudFramework.UI.Rendering.ITextBoard.FixedSize>.

#### Dynamic Text Manipulation

Because it extends <xref:RichHudFramework.UI.Rendering.ITextBuilder>, this interface allows for the modification of text without generating significant garbage. Developers can append, insert, or clear text segments efficiently, making it suitable for high-frequency updates (e.g., debug stats or chat logs). It also provides access to structural metadata through <xref:RichHudFramework.UI.Rendering.ILine> and <xref:RichHudFramework.UI.Rendering.IRichChar>.

#### Navigation and Interaction

`ITextBoard` supplies the coordinate systems required for interactivity:
* **Scrolling:** The <xref:RichHudFramework.UI.Rendering.ITextBoard.TextOffset> property allows the visible window to be panned across the <xref:RichHudFramework.UI.Rendering.ITextBoard.TextSize>.
* **Caret Navigation:** <xref:RichHudFramework.UI.Rendering.ITextBoard.MoveToChar*> calculates the offset required to bring a specific character index into view.
* **Hit Testing:** <xref:RichHudFramework.UI.Rendering.ITextBoard.GetCharAtOffset*> translates a screen-space position into a character index, essential for mouse selection and cursor placement.

#### Usage Note
While the text elements in the built-in UI library (like <xref:RichHudFramework.UI.Label>) do not include scrollbars by default, `ITextBoard` provides the underlying infrastructure to implement them manually. See the examples above for a standard implementation pattern.

---
uid: RichHudFramework.UI.Rendering.ITextBoard
example: [*content]
---
#### Example 1: Efficient Text Updates
The following example demonstrates how to rebuild text in an update loop using the <xref:RichHudFramework.UI.Rendering.ITextBuilder> interface methods inherited by the board. This pattern is often cleaner than manipulating <xref:System.Text.StringBuilder> or <xref:RichHudFramework.UI.RichText> objects.

```csharp
public class TextBuilderExample : HudElementBase
{
    private static readonly List<string> values = new List<string> { "Value 1", "Value 2", "Value 3", "Value 4" };
    private readonly Label textElement;

    // ... Initialization code ...

    protected override void Layout()
    {
        // Access the board directly to manipulate the buffer
        ITextBuilder text = textElement.TextBoard;
        text.Clear();

        // Rebuild the list
        foreach (string val in values)
        {
            text.Append(val);
            text.Append('\n');
        }
    }
}
```

#### Example 2: Implementing a Scrollable Text Box
This example demonstrates how to wrap a <xref:RichHudFramework.UI.TextBox> and a <xref:RichHudFramework.UI.ScrollBar> inside a basic <xref:RichHudFramework.UI.HudElementBase>. It synchronizes the slider's value with the board's <xref:RichHudFramework.UI.Rendering.ITextBoard.TextOffset>.

```csharp
public class ScrollingTextBox : HudElementBase
{
    private readonly TextBox text;
    private readonly ScrollBar vScroll;
    private const string LoremIpsum = "Arbitrary long text..."; 

    public ScrollingTextBox(HudParentBase parent = null) : base(parent)
    {
        text = new TextBox() 
        { 
            AutoResize = false, 
            VertCenterText = false,
            Text = LoremIpsum, 
            BuilderMode = TextBuilderModes.Wrapped
        };

        vScroll = new ScrollBar() 
        { 
            Vertical = true, 
            // Hook the slider event to update the text offset
            UpdateValueCallback = OnScrollUpdate 
        };

        // Layout: Text takes available width (1f), ScrollBar takes fixed width
        var layout = new HudChain(this)
        {
            AlignVertical = false,
            DimAlignment = DimAlignments.UnpaddedSize,
            SizingMode = HudChainSizingModes.FitMembersOffAxis,
            CollectionContainer = { { text, 1f }, vScroll }
        };

        UnpaddedSize = new Vector2(300, 200);
    }

    protected override void Layout()
    {
        // 1. Synchronize the slider if the text offset changes externally (e.g., caret movement)
        vScroll.Value = text.TextBoard.TextOffset.Y; 

        // 2. Calculate the maximum scrollable range (Total Size - Visible Size)
        float maxOffset = Math.Max(0f, text.TextBoard.TextSize.Y - text.TextBoard.Size.Y);
        vScroll.Max = maxOffset;

        // 3. Update the slider handle size to reflect the visible proportion of text
        if (text.TextBoard.TextSize.Y > 0)
            vScroll.VisiblePercent = (text.TextBoard.Size.Y / text.TextBoard.TextSize.Y);
    }

    // Apply the slider value to the text board's Y offset
    private void OnScrollUpdate(object sender, EventArgs args) =>
        text.TextBoard.TextOffset = new Vector2(0f, vScroll.Value);
}
```
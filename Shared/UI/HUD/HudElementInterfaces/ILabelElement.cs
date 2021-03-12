namespace RichHudFramework.UI
{
    using Rendering;

    public interface ILabelElement
    {
        /// <summary>
        /// Text rendered by the label.
        /// </summary>
        RichText Text { get; set; }

        /// <summary>
        /// TextBoard backing the label element.
        /// </summary>
        ITextBoard TextBoard { get; }

        /// <summary>
        /// Default formatting used by the label.
        /// </summary>
        GlyphFormat Format { get; set; }

        /// <summary>
        /// Line formatting mode used by the label.
        /// </summary>
        TextBuilderModes BuilderMode { get; set; }

        /// <summary>
        /// If true, the element will automatically resize to fit the text.
        /// </summary>
        bool AutoResize { get; set; }

        /// <summary>
        /// If true, the text will be vertically centered.
        /// </summary>
        bool VertCenterText { get; set; }

        float Height { get; set; }

        float Width { get; set; }
    }
}

using DeleteBoilerplate.Infrastructure.Models;
using Newtonsoft.Json;

namespace DeleteBoilerplate.GenericComponents.Models.InlineEditors
{
    public class RichTextEditorViewModel : BaseInlineEditorViewModel
    {
        public RichTextEditorViewModel()
        {
            Styles = DefaultStyles;
        }

        public string Text { get; set; }

        public bool EnableFormatting { get; set; } = true;

        public string PlaceholderText { get; set; } = "Please, type here...";

        public CKEditorStyle[] Styles { get; set; }

        public string StylesJson => JsonConvert.SerializeObject(Styles);

        public static CKEditorStyle[] DefaultStyles => new[]
        {
            new CKEditorStyle
            {
                Name = "Suptitle", Element = "div",
                Attribute = new CKEditorAttribute { Class = "content-block__suptitle initial-mb-10" }
            },
            new CKEditorStyle
            {
                Name = "Title", Element = "h2",
                Attribute = new CKEditorAttribute { Class = "content-block__title initial-mb-10 tablet-mb-25" }
            },
            new CKEditorStyle
            {
                Name = "Subtitle", Element = "div",
                Attribute = new CKEditorAttribute { Class = "content-block__subtitle initial-mb-30" }
            },
            new CKEditorStyle
            {
                Name = "Text", Element = "div",
                Attribute = new CKEditorAttribute { Class = "content-block__text" }
            },
            new CKEditorStyle
            {
                Name = "Link", Element = "a",
                Attribute = new CKEditorAttribute { Class = "btn-frameless btn-frameless--type-2" }
            }
        };
    }

}

public class CKEditorStyle
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("element")]
    public string Element { get; set; }

    [JsonProperty("attributes")]
    public CKEditorAttribute Attribute { get; set; }
}

public class CKEditorAttribute
{
    [JsonProperty("class")]
    public string Class { get; set; }
}
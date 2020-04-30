using DeleteBoilerplate.Infrastructure.Models;
using Kentico.Forms.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc;

namespace DeleteBoilerplate.GenericComponents.Models.Widgets.TwitterWidget
{
    public class TwitterWidgetProperties : BaseWidgetViewModel, IWidgetProperties
    {
        [EditingComponent(TextInputComponent.IDENTIFIER, Label = "Title", Order = 100, DefaultValue= "Latest from our Twitter")]
        public string Title { get; set; }

        [EditingComponent(TextInputComponent.IDENTIFIER, Label = "Screen name", Order = 200)]
        public string ScreenName { get; set; }

        [EditingComponent(IntInputComponent.IDENTIFIER, Label = "Tweet count", Order = 300, DefaultValue = 10)]
        public int TweetCount { get; set; }
    }
}
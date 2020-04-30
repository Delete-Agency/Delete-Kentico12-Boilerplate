using DeleteBoilerplate.Infrastructure.Models;
using DeleteBoilerplate.Twitter.Extensions;

namespace DeleteBoilerplate.GenericComponents.Models.Widgets.TwitterWidget
{
    public class TwitterWidgetViewModel : BaseWidgetViewModel
    {
        public string Title { get; set; }

        public string ProfileLink => TwitterExtensions.GetProfileLink(ScreenName);

        public string ScreenName { get; set; }

        public int TweetCount { get; set; }

        public string Signet { get; set; }

        public string GetTweetsMainPartApi { get; set; }

        public string GetTweetsApi => $"{GetTweetsMainPartApi}?screenName={ScreenName}&tweetCount={TweetCount}&signet={Signet}";
    }
}
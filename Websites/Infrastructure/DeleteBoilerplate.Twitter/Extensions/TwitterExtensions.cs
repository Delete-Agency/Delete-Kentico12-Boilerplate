using LinqToTwitter;

namespace DeleteBoilerplate.Twitter.Extensions
{
    public static class TwitterExtensions
    {
        public static string GetTweetLink(this Status status)
        {
            return $"https://twitter.com/{status.ScreenName}/status/{status.StatusID}";
        }

        public static string GetProfileLink(string screenName)
        {
            return $"https://twitter.com/{screenName}";
        }
    }
}
using CMS.EventLog;
using LinqToTwitter;
using System;
using System.Collections.Generic;
using System.Linq;
namespace DeleteBoilerplate.Twitter.Services
{
    public interface ITwitterService
    {
        IList<Status> GetTweetStatusList(string screenName, int tweetCount);
    }

    public class TwitterService : ITwitterService
    {
        protected TwitterContext TwitterContext { get; }

        public TwitterService(TwitterContext twitterContext)
        {
            TwitterContext = twitterContext;
        }

        public IList<Status> GetTweetStatusList(string screenName, int tweetCount)
        {
            try
            {
                var tweetStatusList = (
                        from tweet in this.TwitterContext.Status
                        where tweet.Type == StatusType.User
                              && tweet.ScreenName == screenName
                              && tweet.Count == tweetCount
                              && tweet.TweetMode == TweetMode.Extended
                              && tweet.IncludeAltText == true
                        select tweet)
                    .ToListAsync();

                return tweetStatusList.GetAwaiter().GetResult(); ;
            }
            catch (Exception e) 
            {
                EventLogProvider.LogException("TwitterService", "TWITTER_ERRORS", e);
                return new List<Status>();
            }
        }
    }
}
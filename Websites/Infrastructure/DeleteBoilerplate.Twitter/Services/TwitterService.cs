using DeleteBoilerplate.Common.Helpers;
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
            catch (Exception ex) 
            {
                LogHelper.LogException(ex);

                return new List<Status>();
            }
        }
    }
}
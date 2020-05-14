using AutoMapper;
using Castle.Core.Internal;
using DeleteBoilerplate.Common.Helpers;
using DeleteBoilerplate.DynamicRouting.Controllers;
using DeleteBoilerplate.GenericComponents.Constants;
using DeleteBoilerplate.GenericComponents.Controllers.Widgets;
using DeleteBoilerplate.GenericComponents.Models.Widgets.TwitterWidget;
using DeleteBoilerplate.Infrastructure.Services;
using DeleteBoilerplate.Twitter.Services;
using Kentico.PageBuilder.Web.Mvc;
using LightInject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

[assembly:
    RegisterWidget("DeleteBoilerplate.GenericComponents.TwitterWidget", typeof(TwitterWidgetController),
        "{$DeleteBoilerplate.GenericComponents.TwitterWidget.Name$}", Description = "{$DeleteBoilerplate.GenericComponents.TwitterWidget.Description$}",
        IconClass = "icon-brand-twitter")]

namespace DeleteBoilerplate.GenericComponents.Controllers.Widgets
{
    public class TwitterWidgetController : BaseWidgetController<TwitterWidgetProperties>
    {
        private const string ControllerName = "TwitterWidget";

        [Inject]
        protected IMapper Mapper { get; set; }

        [Inject]
        protected ITwitterService TwitterService { get; set; }

        [Inject]
        protected IHashService HashService { get; set; }

        public ActionResult Index()
        {
            var properties = GetProperties();

            if (properties.ScreenName.IsNullOrEmpty())
                return null;

            var model = Mapper.Map<TwitterWidgetViewModel>(properties);
            model.Signet = this.HashService.GetHash(model.ScreenName);
            model.GetTweetsMainPartApi = Url.RouteUrl(RouteNames.Twitter.GetTweets);

            return PartialView("Widgets/_Twitter", model);
        }

        [HttpGet]
        [OutputCache(Duration = 2 * 60 * 60, VaryByParam = "screenName;signet;tweetCount")]
        public JsonResult GetTweets(string screenName, string signet, int tweetCount = 10)
        {
            try
            {
                bool isValidRequest = this.HashService.ValidateHash(screenName, signet);
                if (isValidRequest)
                {
                    var tweetStatusList = this.TwitterService.GetTweetStatusList(screenName, tweetCount);
                    var tweets = Mapper.Map<List<TweetViewModel>>(tweetStatusList);

                    if (tweets?.Any() == true)
                    {
                        return JsonSuccess(tweets);
                    }
                }

                return JsonError("Invalid request.");
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex);

                return JsonError(ex.ToString());
            }
        }
    }
}
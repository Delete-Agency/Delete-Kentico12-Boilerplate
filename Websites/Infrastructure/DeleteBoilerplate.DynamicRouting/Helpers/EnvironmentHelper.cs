using System;
using System.Security.Principal;
using System.Web;
using CMS.Base;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using Kentico.Content.Web.Mvc;
using Kentico.Web.Mvc;

namespace DeleteBoilerplate.DynamicRouting.Helpers
{
    public class EnvironmentHelper
    {
        /// <summary>
        /// Returns if the Preview is Enabled or not
        /// </summary>
        public static bool PreviewEnabled
        {
            get
            {
                try
                {
                    return HttpContext.Current.Kentico().Preview().Enabled;
                }
                catch (InvalidOperationException)
                {
                    // This occurs only on the Owin Authentication calls due to the Dynamic route handler
                    return false;
                }
            }
        }

        /// <summary>
        /// Gets the Preview Culture
        /// </summary>
        public static string PreviewCulture
        {
            get
            {
                try
                {
                    return HttpContext.Current.Kentico().Preview().CultureName;
                }
                catch (InvalidOperationException)
                {
                    // This occurs only on the Owin Authentication calls due to the Dynamic route handler
                    return "en-US";
                }
            }
        }

        /// <summary>
        /// Uses MembershipContext.AuthenticatedUser
        /// </summary>
        /// <returns></returns>
        public static UserInfo AuthenticatedUser()
        {
            return MembershipContext.AuthenticatedUser;
        }

        /// <summary>
        /// Uses the given IPrincipleUser to get the current user.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static UserInfo AuthenticatedUser(IPrincipal user)
        {
            var username = (user?.Identity != null ? user.Identity.Name : "public");
            return CacheHelper.Cache(cs => UserInfoProvider.GetUserInfo(username), new CacheSettings(CacheHelper.CacheMinutes(SiteContext.CurrentSiteName), "AuthenticatedUser", username));
        }

        /// <summary>
        /// Gets the Url requested, handling any Virtual Directories
        /// </summary>
        /// <param name="request">The Request</param>
        /// <returns>The Url for lookup</returns>
        public static string GetUrl(HttpRequestBase request)
        {
            return GetUrl(request.Url.AbsolutePath, request.ApplicationPath);
        }

        /// <summary>
        /// Gets the Url requested, handling any Virtual Directories
        /// </summary>
        /// <param name="request">The Request</param>
        /// <returns>The Url for lookup</returns>
        public static string GetUrl(HttpRequest request)
        {
            return GetUrl(request.Url.AbsolutePath, request.ApplicationPath);
        }

        /// <summary>
        /// Gets the Url requested, handling any Virtual Directories
        /// </summary>
        /// <param name="request">The Request</param>
        /// <returns>The Url for lookup</returns>
        public static string GetUrl(IRequest request)
        {
            return GetUrl(request.Url.AbsolutePath, HttpContext.Current.Request.ApplicationPath);
        }

        /// <summary>
        /// Removes Application Path from Url if present and ensures starts with /
        /// </summary>
        /// <param name="relativeUrl">The Url (Relative)</param>
        /// <param name="applicationPath"></param>
        /// <returns></returns>
        public static string GetUrl(string relativeUrl, string applicationPath)
        {
            // Remove Application Path from Relative Url if it exists at the beginning
            if (applicationPath != "/" && string.IsNullOrWhiteSpace(applicationPath) && relativeUrl.ToLower().IndexOf(applicationPath.ToLower()) == 0)
            {
                relativeUrl = relativeUrl.Substring(applicationPath.Length);
            }

            return "/" + relativeUrl.Trim("/~".ToCharArray()).Split("?#:".ToCharArray())[0];
        }

    }
}
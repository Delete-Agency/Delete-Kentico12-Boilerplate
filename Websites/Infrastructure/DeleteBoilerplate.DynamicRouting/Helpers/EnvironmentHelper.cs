using System;
using System.Web;

namespace DeleteBoilerplate.DynamicRouting.Helpers
{
    public class EnvironmentHelper
    {
        /// <summary>
        /// Gets the Url requested, handling any Virtual Directories
        /// </summary>
        /// <param name="request">The Request</param>
        /// <returns>The Url for lookup</returns>
        public static string GetUrl(HttpRequestBase request)
        {
            return GetUrl(request?.Url?.AbsolutePath, request?.ApplicationPath);
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
        /// Removes Application Path from Url if present and ensures starts with /
        /// </summary>
        /// <param name="relativeUrl">The Url (Relative)</param>
        /// <param name="applicationPath"></param>
        /// <returns></returns>
        public static string GetUrl(string relativeUrl, string applicationPath)
        {
            // Remove Application Path from Relative Url if it exists at the beginning
            if (applicationPath != "/" && !string.IsNullOrWhiteSpace(applicationPath) && relativeUrl.ToLower().IndexOf(applicationPath.ToLower(), StringComparison.OrdinalIgnoreCase) == 0)
            {
                relativeUrl = relativeUrl.Substring(applicationPath.Length);
            }

            return "/" + relativeUrl.Trim("/~".ToCharArray()).Split("?#:".ToCharArray())[0];
        }
    }
}
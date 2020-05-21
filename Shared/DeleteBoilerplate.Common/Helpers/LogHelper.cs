using CMS.EventLog;
using CMS.SiteProvider;
using System;
using System.Linq;
using System.Web;

namespace DeleteBoilerplate.Common.Helpers
{
    public static class LogHelper
    {
        private const int SymbolsLimitInDB = 100;

        public static void LogInformation(string source, string evenCode, string description = "", string parameters = "", int siteId = 0)
        {
            var validSource = GetValidSource(source);
            var validEventCode = GetValidEventCode(evenCode);
            var validSiteId = GetValidSiteId(siteId);

            EventLogProvider.LogInformation(validSource, validEventCode, $"{description} [SiteId: {validSiteId}, {parameters}]" );
        }

        public static void LogError(string source, string evenCode, string description = "", int siteId = 0)
        {
            var validSource = GetValidSource(source);
            var validEventCode = GetValidEventCode(evenCode);
            var validSiteId = GetValidSiteId(siteId);

            EventLogProvider.LogEvent(EventType.ERROR, validSource, validEventCode, description, siteId: validSiteId);
        }

        public static void LogException(string source, string evenCode, Exception ex, string message = "", int siteId = 0)
        {
            AddLogException(source, evenCode, ex, siteId, message);
        }

        public static void LogException(Exception ex, int siteId = 0)
        {
            AddLogException($"{ex.TargetSite?.DeclaringType?.Name}.{ex.TargetSite?.Name}()", ex.Message, ex, siteId);
        }

        public static void LogException(string eventCode, Exception ex, int siteId = 0)
        {
            AddLogException($"{ex.TargetSite?.DeclaringType?.Name}.{ex.TargetSite?.Name}()", eventCode, ex, siteId);
        }

        public static void LogException(HttpRequestBase request, Exception ex, int siteId = 0)
        {
            var controllerName = GetControllerAndActionName(request, out string actionName);

            AddLogException($"{controllerName}.{actionName}()", ex.Message, ex, siteId);
        }

        public static void LogException(HttpRequestBase request, string evenCode, Exception ex, int siteId = 0)
        {
            var controllerName = GetControllerAndActionName(request, out string actionName);

            AddLogException($"{controllerName}.{actionName}()", evenCode, ex, siteId);
        }

        private static string GetControllerAndActionName(HttpRequestBase request, out string actionName)
        {
            var controllerName = request.RequestContext.RouteData.Values["controller"]?.ToString() ?? string.Empty;
            actionName = request.RequestContext.RouteData.Values["action"]?.ToString() ?? string.Empty;
            
            return controllerName;
        }

        private static void AddLogException(string source, string evenCode, Exception ex, int siteId = 0, string message = "")
        {
            var validSource = GetValidSource(source);
            var validEventCode = GetValidEventCode(evenCode);
            var validSiteId = GetValidSiteId(siteId);

            EventLogProvider.LogException(validSource, validEventCode, ex, validSiteId, message);
        }

        private static int GetValidSiteId(int siteId = 0)
        {
            return (siteId == 0)
                ? SiteContext.CurrentSiteID
                : siteId;
        }

        private static string GetValidSource(string source)
        {
            return (source.Length > SymbolsLimitInDB)
                ? new string(source.Take(SymbolsLimitInDB).ToArray())
                : source;
        }

        private static string GetValidEventCode(string eventCode)
        {
            return GetValidSource(eventCode);
        }
    }
}

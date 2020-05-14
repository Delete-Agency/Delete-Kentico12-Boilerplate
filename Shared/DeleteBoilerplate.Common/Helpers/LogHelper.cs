using System;
using System.Web;
using CMS.EventLog;

namespace DeleteBoilerplate.Common.Helpers
{
    public static class LogHelper
    {
        public static void LogError(string source, string evenCode, string description)
        {
            EventLogProvider.LogEvent(EventType.ERROR, source, evenCode, description);
        }

        public static void LogError(string source, string evenCode, string description, int siteId)
        {
            EventLogProvider.LogEvent(EventType.ERROR, source, evenCode, description, siteId: siteId);
        }

        public static void LogException(Exception ex)
        {
            EventLogProvider.LogException($"{ex.TargetSite?.DeclaringType?.Name}.{ex.TargetSite?.Name}()", ex.Message, ex);
        }

        public static void LogException(string eventCode, Exception ex)
        {
            EventLogProvider.LogException($"{ex.TargetSite?.DeclaringType?.Name}.{ex.TargetSite?.Name}()", eventCode, ex);
        }

        public static void LogException(string eventCode, Exception ex, int siteId)
        {
            EventLogProvider.LogException($"{ex.TargetSite?.DeclaringType?.Name}.{ex.TargetSite?.Name}()", eventCode, ex, siteId);
        }

        public static void LogException(string source, string evenCode, Exception ex)
        {
            EventLogProvider.LogException(source, evenCode, ex);
        }

        public static void LogException(HttpRequestBase request, Exception ex)
        {
            var controllerName = GetControllerAndActionName(request, out string actionName);

            EventLogProvider.LogException($"{controllerName}.{actionName}()", ex.Message, ex);
        }

        public static void LogException(HttpRequestBase request, string evenCode, Exception ex)
        {
            var controllerName = GetControllerAndActionName(request, out string actionName);

            EventLogProvider.LogException($"{controllerName}.{actionName}()", evenCode, ex);
        }

        private static string GetControllerAndActionName(HttpRequestBase request, out string actionName)
        {
            var controllerName = request.RequestContext.RouteData.Values["controller"]?.ToString() ?? string.Empty;
            actionName = request.RequestContext.RouteData.Values["action"]?.ToString() ?? string.Empty;
            
            return controllerName;
        }
    }
}

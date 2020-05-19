using System;
using System.Linq;
using System.Web;
using CMS.EventLog;

namespace DeleteBoilerplate.Common.Helpers
{
    public static class LogHelper
    {
        private const int SymbolsLimitInDB = 100;

        public static void LogError(string source, string evenCode, string description, int siteId = 0)
        {
            EventLogProvider.LogEvent(EventType.ERROR, source, evenCode, description, siteId: siteId);
        }

        public static void LogException(Exception ex, int siteId = 0)
        {
           AddLogException($"{ex.TargetSite?.DeclaringType?.Name}.{ex.TargetSite?.Name}()", ex.Message, ex, siteId);
        }

        public static void LogException(string eventCode, Exception ex, int siteId = 0)
        {
           AddLogException($"{ex.TargetSite?.DeclaringType?.Name}.{ex.TargetSite?.Name}()", eventCode, ex, siteId);
        }

        public static void LogException(string source, string evenCode, Exception ex, int siteId = 0)
        {
            AddLogException(source, evenCode, ex, siteId);
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

        private static void AddLogException(string source, string evenCode, Exception ex, int siteId = 0)
        {
            var validSource = new string(source.Take(SymbolsLimitInDB).ToArray());
            var validEventCode = new string(evenCode.Take(SymbolsLimitInDB).ToArray());
        
            EventLogProvider.LogException(validSource, validEventCode, ex, siteId);
        }
    }
}

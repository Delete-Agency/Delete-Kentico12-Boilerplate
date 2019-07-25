using System.Collections.Generic;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using DeleteBoilerplate.DynamicRouting.Helpers;
using RequestContext = System.Web.Routing.RequestContext;

namespace DeleteBoilerplate.DynamicRouting.RequestHandling
{
    public class DynamicHttpHandler : IHttpHandler
    {
        public DynamicHttpHandler(RequestContext requestContext, Dictionary<string, MethodInfo> routingDictionary)
        {
            this.RequestContext = requestContext;
            this._routingDictionary = routingDictionary;
        }

        public RequestContext RequestContext { get; set; }

        private readonly Dictionary<string, MethodInfo> _routingDictionary;

        public bool IsReusable => false;

        public void ProcessRequest(HttpContext context)
        {
            var factory = ControllerBuilder.Current.GetControllerFactory();

            var defaultController = RequestContext.RouteData.Values.ContainsKey("controller") ? RequestContext.RouteData.Values["controller"].ToString() : "";
            var defaultAction = RequestContext.RouteData.Values.ContainsKey("action") ? RequestContext.RouteData.Values["action"].ToString() : "";

            // Get the classname based on the URL
            var foundNode = DocumentQueryHelper.GetNodeByAliasPathOrSeoUrl(EnvironmentHelper.GetUrl(context.Request));
            var className = foundNode.ClassName;

            _routingDictionary.TryGetValue(className, out var controllerMethod);

            var controllerName = controllerMethod?.ReflectedType?.Name;
            var controllerAction = controllerMethod?.Name;

            if (string.IsNullOrWhiteSpace(controllerName) || string.IsNullOrWhiteSpace(controllerAction))
            {
                controllerName = defaultController;
                controllerAction = defaultAction;
            }

            controllerName = controllerName.Replace("Controller", string.Empty);

            this.RequestContext.RouteData.Values["Controller"] = controllerName;
            this.RequestContext.RouteData.Values["Action"] = controllerAction;

            var controller = factory.CreateController(this.RequestContext, controllerName);

            controller.Execute(this.RequestContext);
            
            factory.ReleaseController(controller);
        }
    }
}
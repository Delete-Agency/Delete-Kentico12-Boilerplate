using System.Web;
using System.Web.Mvc;
using DeleteBoilerplate.DynamicRouting.Config;
using RequestContext = System.Web.Routing.RequestContext;

namespace DeleteBoilerplate.DynamicRouting.RequestHandling
{
    public class DynamicHttpHandler : IHttpHandler
    {
        public DynamicHttpHandler(RequestContext httpRequestContext)
        {
            this.HttpRequestContext = httpRequestContext;
        }

        public RequestContext HttpRequestContext { get; set; }
        
        public bool IsReusable => false;

        public void ProcessRequest(HttpContext context)
        {
            var factory = ControllerBuilder.Current.GetControllerFactory();

            var defaultController = this.HttpRequestContext.RouteData.Values.ContainsKey("controller") ? this.HttpRequestContext.RouteData.Values["controller"].ToString() : "";
            var defaultAction = this.HttpRequestContext.RouteData.Values.ContainsKey("action") ? this.HttpRequestContext.RouteData.Values["action"].ToString() : "";

            var className = context.Items["ContextItemClassName"]?.ToString();
            var controllerMethod = PageTypeRoutingConfig.GetTargetControllerMethod(className);

            var controllerName = controllerMethod?.ReflectedType?.Name;
            var controllerAction = controllerMethod?.Name;

            if (string.IsNullOrWhiteSpace(controllerName) || string.IsNullOrWhiteSpace(controllerAction))
            {
                controllerName = defaultController;
                controllerAction = defaultAction;
            }

            controllerName = controllerName.Replace("Controller", string.Empty);

            this.HttpRequestContext.RouteData.Values["Controller"] = controllerName;
            this.HttpRequestContext.RouteData.Values["Action"] = controllerAction;

            this.HttpRequestContext.HttpContext = new HttpContextWrapper(context);

            var controller = factory.CreateController(this.HttpRequestContext, controllerName);

            controller.Execute(this.HttpRequestContext);
            
            factory.ReleaseController(controller);
        }
    }
}
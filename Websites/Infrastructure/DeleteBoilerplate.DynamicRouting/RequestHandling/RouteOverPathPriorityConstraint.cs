using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using CMS.Helpers;
using RequestContext = System.Web.Routing.RequestContext;

namespace DeleteBoilerplate.DynamicRouting.RequestHandling
{
    public class RouteOverPathPriorityConstraint : IRouteConstraint
    {
        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            // Check if the controller is found and has the RouteOverPathPriority attribute.
            var controllerName = (values.ContainsKey("controller") ? values["controller"].ToString() : "");
            return CacheHelper.Cache(cs =>
            {
                // Check if the Route that it found has the override
                var factory = ControllerBuilder.Current.GetControllerFactory();
                try
                {
                    var controller = factory.CreateController(new RequestContext(httpContext, new RouteData(route, null)), controllerName);
                    return Attribute.GetCustomAttribute(controller.GetType(), typeof(RouteOverPathPriority)) != null;
                }
                catch (Exception)
                {
                    return false;
                }
            }, new CacheSettings(1440, "RouteOverPathPriority", controllerName));
        }
    }
}
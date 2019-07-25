using System.Collections.Generic;
using System.Reflection;
using System.Web;
using System.Web.Routing;

namespace DeleteBoilerplate.DynamicRouting.RequestHandling
{
    public class DynamicRouteHandler : IRouteHandler
    {
        private readonly Dictionary<string, MethodInfo> _routingDictionary;

        public DynamicRouteHandler(Dictionary<string, MethodInfo> routingDictionary)
        {
            this._routingDictionary = routingDictionary;
        }

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new DynamicHttpHandler(requestContext, this._routingDictionary);
        }
    }
}
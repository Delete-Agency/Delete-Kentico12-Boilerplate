using System.Web;
using System.Web.Routing;

namespace DeleteBoilerplate.DynamicRouting.RequestHandling
{
    public class DynamicRouteHandler : IRouteHandler
    {
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new DynamicHttpHandler(requestContext);
        }
    }
}
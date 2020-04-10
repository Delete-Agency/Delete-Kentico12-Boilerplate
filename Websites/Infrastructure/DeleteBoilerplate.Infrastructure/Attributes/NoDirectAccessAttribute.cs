using System;
using System.Web.Mvc;
using DeleteBoilerplate.Domain;

namespace DeleteBoilerplate.Infrastructure.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class NoDirectAccessAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            var actionName = filterContext.ActionDescriptor.ActionName;

            var url = $"/{controllerName}/{actionName}";

            if (string.Equals(filterContext.HttpContext?.Request.Url?.LocalPath, url, StringComparison.OrdinalIgnoreCase))
            {
                filterContext.HttpContext?.Server.TransferRequest(Settings.Pages.NotFoundPagePath);
            }
        }
    }
}
using System.Web.Mvc;
using AutoMapper;
using CMS.Base;
using LightInject;
using IRequestContext = DeleteBoilerplate.DynamicRouting.Contexts.IRequestContext;

namespace DeleteBoilerplate.DynamicRouting.Controllers
{
    public class BaseCustomObjectController : Controller
    {
        [Inject]
        protected IMapper Mapper { get; set; }

        [Inject]
        protected IRequestContext RequestContext { get; set; }

        protected virtual void ResolveContext(ICMSObject contextItem)
        {
            this.RequestContext.ContextItem = contextItem;
            this.RequestContext.IsContextItemResolved = true;
            this.RequestContext.IsContextResolved = true;
        }
    }
}
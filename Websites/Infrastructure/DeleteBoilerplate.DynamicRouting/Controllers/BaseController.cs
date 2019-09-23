using System.Web.Mvc;
using CMS.DocumentEngine;
using DeleteBoilerplate.Domain;
using DeleteBoilerplate.OutputCache;
using Kentico.Content.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc;
using Kentico.Web.Mvc;
using LightInject;
using IRequestContext = DeleteBoilerplate.DynamicRouting.Contexts.IRequestContext;

namespace DeleteBoilerplate.DynamicRouting.Controllers
{
    public class BaseController : Controller
    {
        [Inject]
        protected IRequestContext RequestContext { get; set; }

        [Inject]
        protected IOutputCacheDependencies OutputCacheDependencies { get; set; }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!this.RequestContext.ContextResolved)
                this.ResolveContext();

            if (this.RequestContext.ContextItemId.HasValue)
            {
                HttpContext.Kentico().PageBuilder().Initialize(RequestContext.ContextItemId.Value);
                OutputCacheDependencies.AddDocumentIdDependency(RequestContext.ContextItemId.Value);
            }

            base.OnActionExecuting(filterContext);
        }

        protected virtual T GetContextItem<T>() where T : TreeNode, new()
        {
            return this.RequestContext.GetContextItem<T>();
        }

        private void ResolveContext()
        {
            this.RequestContext.ContextItemId = this.HttpContext.Items[Constants.DynamicRouting.ContextItemDocumentId] as int?;
            this.RequestContext.IsPreview = this.HttpContext.Kentico().Preview().Enabled;
            this.RequestContext.ContextResolved = true;
        }
    }
}
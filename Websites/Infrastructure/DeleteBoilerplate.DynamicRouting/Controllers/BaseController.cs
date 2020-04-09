using CMS.DocumentEngine;
using DeleteBoilerplate.Domain;
using DeleteBoilerplate.Domain.Extensions;
using DeleteBoilerplate.OutputCache;
using Kentico.PageBuilder.Web.Mvc;
using Kentico.Web.Mvc;
using LightInject;
using System.Linq;
using System.Web.Mvc;
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
            if (!this.RequestContext.IsContextResolved)
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
            if (!this.RequestContext.IsContextItemResolved)
                this.ResolveContextItem<T>();

            if (this.RequestContext.ContextItem is T typedContextItem)
                return typedContextItem;

            return null;
        }

        private void ResolveContext()
        {
            this.RequestContext.ContextItemId = this.HttpContext.Items[Constants.DynamicRouting.ContextItemDocumentId] as int?;
            this.RequestContext.IsContextResolved = true;
        }

        private void ResolveContextItem<T>() where T : TreeNode, new()
        {
            if (this.RequestContext.ContextItemId.HasValue)
            {
                T contextItem = DocumentHelper.GetDocuments<T>()
                        .WithID(this.RequestContext.ContextItemId.Value)
                        .TopN(1)
                        .AddVersionsParameters(Settings.PreviewEnabled)
                        .FirstOrDefault();

                this.RequestContext.ContextItem = contextItem;
                this.RequestContext.IsContextItemResolved = true;
            }
        }
    }
}
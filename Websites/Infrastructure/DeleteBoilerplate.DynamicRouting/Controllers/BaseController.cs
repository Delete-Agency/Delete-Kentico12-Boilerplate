using System.Linq;
using System.Web.Mvc;
using CMS.DocumentEngine;
using DeleteBoilerplate.DynamicRouting.Extensions;
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

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!this.RequestContext.ContextResolved)
                this.ResolveContext();

            if (this.RequestContext.ContextItemId.HasValue)
                HttpContext.Kentico().PageBuilder().Initialize(this.RequestContext.ContextItemId.Value);

            base.OnActionExecuting(filterContext);
        }

        protected virtual T GetContextItem<T>() where T: TreeNode, new()
        {
            if (!this.RequestContext.ContextItemResolved)
                this.ResolveContextItem<T>();

            if (this.RequestContext.ContextItem is T typedContextItem)
                return typedContextItem;

            return null;
        }

        private void ResolveContext()
        {
            this.RequestContext.ContextItemId = this.HttpContext.Items["ContextItemDocumentId"] as int?;
            this.RequestContext.IsPreview = this.HttpContext.Kentico().Preview().Enabled;
        }

        private void ResolveContextItem<T>() where T: TreeNode, new()
        {
            if (this.RequestContext.ContextItemId.HasValue)
            {
                var query = DocumentHelper.GetDocuments<T>()
                    .WithID(this.RequestContext.ContextItemId.Value)
                    .TopN(1)
                    .AddVersionsParameters(this.RequestContext.IsPreview);

                this.RequestContext.ContextItem = query.FirstOrDefault();
                this.RequestContext.ContextItemResolved = true;
            }
        }
    }
}
using System.Linq;
using CMS.DocumentEngine;
using DeleteBoilerplate.Domain.Extensions;

namespace DeleteBoilerplate.DynamicRouting.Contexts.Implementation
{
    public class RequestContext : IRequestContext
    {
        public int? ContextItemId { get; set; }

        public bool ContextResolved { get; set; }

        public bool IsPreview { get; set; }

        private TreeNode ContextItem { get; set; }

        private bool ContextItemResolved { get; set; }

        public T GetContextItem<T>() where T : TreeNode, new()
        {
            if (!this.ContextItemResolved)
                this.ResolveContextItem<T>();

            if (this.ContextItem is T typedContextItem)
                return typedContextItem;

            return null;
        }

        private void ResolveContextItem<T>() where T : TreeNode, new()
        {
            if (this.ContextItemId.HasValue)
            {
                var query = DocumentHelper.GetDocuments<T>()
                    .WithID(this.ContextItemId.Value)
                    .TopN(1)
                    .AddVersionsParameters(this.IsPreview);

                this.ContextItem = query.FirstOrDefault();
                this.ContextItemResolved = true;
            }
        }
    }
}
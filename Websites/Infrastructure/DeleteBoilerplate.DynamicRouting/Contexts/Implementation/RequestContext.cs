using CMS.DocumentEngine;

namespace DeleteBoilerplate.DynamicRouting.Contexts.Implementation
{
    public class RequestContext : IRequestContext
    {
        public int? ContextItemId { get; set; }

        public TreeNode ContextItem { get; set; }

        public bool ContextItemResolved { get; set; }

        public bool ContextResolved { get; set; }

        public bool IsPreview { get; set; }
    }
}
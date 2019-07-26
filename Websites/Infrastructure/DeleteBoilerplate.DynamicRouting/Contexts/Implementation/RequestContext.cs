using CMS.DocumentEngine;

namespace DeleteBoilerplate.DynamicRouting.Contexts.Implementation
{
    public class RequestContext : IRequestContext
    {
        public TreeNode ContextItem { get; set; }

        public bool ContextResolved { get; set; }

        public bool IsPreview { get; set; }
    }
}
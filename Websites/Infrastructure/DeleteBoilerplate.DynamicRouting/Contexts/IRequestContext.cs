using CMS.DocumentEngine;

namespace DeleteBoilerplate.DynamicRouting.Contexts
{
    public interface IRequestContext
    {
        TreeNode ContextItem { get; set; }

        bool ContextResolved { get; set; }

        bool IsPreview { get; set; }
    }
}

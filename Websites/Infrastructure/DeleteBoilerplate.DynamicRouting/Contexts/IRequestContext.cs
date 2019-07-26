using CMS.DocumentEngine;

namespace DeleteBoilerplate.DynamicRouting.Contexts
{
    public interface IRequestContext
    {
        int? ContextItemId { get; set; }

        TreeNode ContextItem { get; set; }

        bool ContextItemResolved { get; set; }

        bool IsPreview { get; set; }
    }
}

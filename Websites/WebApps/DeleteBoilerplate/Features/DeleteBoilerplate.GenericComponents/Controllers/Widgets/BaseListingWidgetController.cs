using CMS.DocumentEngine;
using DeleteBoilerplate.DynamicRouting.Controllers;
using DeleteBoilerplate.GenericComponents.Models.Widgets;

namespace DeleteBoilerplate.GenericComponents.Controllers.Widgets
{
    public abstract class BaseListingWidgetController<TProperties, TTreeNode> : BaseWidgetController<TProperties> 
        where TProperties: BaseListingWidgetProperties<TTreeNode>, new()
        where TTreeNode: TreeNode, new()
    {
    }
}
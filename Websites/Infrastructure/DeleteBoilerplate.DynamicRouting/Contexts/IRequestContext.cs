using CMS.Base;
using CMS.DocumentEngine;

namespace DeleteBoilerplate.DynamicRouting.Contexts
{
    public interface IRequestContext
    {
        int? ContextItemId { get; set; }

        ICMSObject ContextItem { get; set; }

        bool IsContextItemResolved { get; set; }

        bool IsContextResolved { get; set; }

    }
}

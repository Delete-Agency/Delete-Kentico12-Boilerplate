using CMS.Base;

namespace DeleteBoilerplate.DynamicRouting.Contexts.Implementation
{
    public class RequestContext : IRequestContext
    {
        public int? ContextItemId { get; set; }

        public ICMSObject ContextItem { get; set; }

        public bool IsContextItemResolved { get; set; }

        public bool IsContextResolved { get; set; }
    }
}
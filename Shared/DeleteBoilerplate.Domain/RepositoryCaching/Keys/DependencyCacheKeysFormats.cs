namespace DeleteBoilerplate.Domain.RepositoryCaching.Keys
{
    public static class DependencyCacheKeysFormats
    {
        /// <summary>
        /// All nodes of page type, {0} - site name, {1} - page type class name
        /// </summary>
        public const string PageType = "nodes|{0}|{1}|all";

        /// <summary>
        /// All object of specific type, {0} - object type name
        /// </summary>
        public const string ObjectType = "{0}|all";

        /// <summary>
        /// Specific page, {0} - site name, {1} - node guid
        /// </summary>
        public const string SpecificPage = "nodeguid|{0}|{1}";

        /// <summary>
        /// Specific cms object, {0} - object type name, {1} - object id
        /// </summary>
        public const string SpecificObject = "{0}|byid|{1}";
    }
}
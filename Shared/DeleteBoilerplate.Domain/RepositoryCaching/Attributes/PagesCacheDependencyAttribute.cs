using System;
using DeleteBoilerplate.Domain.RepositoryCaching.Keys;

namespace DeleteBoilerplate.Domain.RepositoryCaching.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class PagesCacheDependencyAttribute : CacheDependencyAttribute
    {
        private readonly string _pageTypeClassName;

        public PagesCacheDependencyAttribute(string pageTypeClassName)
            : base(DependencyCacheKeysFormats.PageType)
        {
            _pageTypeClassName = pageTypeClassName;
        }

        internal override string ResolveKey(string siteName, object[] methodArguments)
        {
            return string.Format(DependencyCacheKeysFormats.PageType, siteName, _pageTypeClassName);
        }
    }
}

using System;

namespace DeleteBoilerplate.Domain.RepositoryCaching.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class CacheDependencyAttribute : System.Attribute
    {
        private readonly string _dependencyKeyFormat;

        public CacheDependencyAttribute(string dependencyKeyFormat)
        {
            if (string.IsNullOrEmpty(dependencyKeyFormat))
            {
                throw new ArgumentException(nameof(dependencyKeyFormat));
            }

            _dependencyKeyFormat = dependencyKeyFormat;
        }

        internal virtual string ResolveKey(string siteName, object[] methodArguments)
        {
            var keyFormatWithSiteName = _dependencyKeyFormat.Replace("##SITENAME##", siteName);
            return string.Format(keyFormatWithSiteName, methodArguments);
        }
    }
}

using System;

namespace DeleteBoilerplate.Domain.RepositoryCaching.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class RepositoryCachingAttribute : System.Attribute
    {
    }
}

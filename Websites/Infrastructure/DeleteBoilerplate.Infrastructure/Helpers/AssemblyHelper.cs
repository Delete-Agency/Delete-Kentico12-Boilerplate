using System;
using System.Linq;
using System.Reflection;

namespace DeleteBoilerplate.Infrastructure.Helpers
{
    public static class AssemblyHelper
    {
        public static Assembly[] GetDiscoverableAsseblyAssemblies() => AppDomain.CurrentDomain.GetAssemblies().Where(x =>
            x.CustomAttributes.Any(ca => ca.AttributeType == typeof(DeleteBoilerplateAssemblyAttribute))).ToArray();
    }
}

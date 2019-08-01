using System;
using System.Linq;
using System.Reflection;
using DeleteBoilerplate.Common;

namespace DeleteBoilerplate.Infrastructure.Helpers
{
    public static class AssemblyHelper
    {
        public static Assembly[] GetDiscoverableAssemblyAssemblies() => AppDomain.CurrentDomain.GetAssemblies().Where(x =>
            x.CustomAttributes.Any(ca => ca.AttributeType == typeof(DeleteBoilerplateAssemblyAttribute))).ToArray();
    }
}

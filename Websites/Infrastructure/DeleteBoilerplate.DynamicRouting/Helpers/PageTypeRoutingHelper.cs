using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using DeleteBoilerplate.DynamicRouting.Attributes;

namespace DeleteBoilerplate.DynamicRouting.Helpers
{
    public static class PageTypeRoutingHelper
    {
        private static readonly Dictionary<string, MethodInfo> RoutingDictionary = new Dictionary<string, MethodInfo>(StringComparer.OrdinalIgnoreCase);

        public static void Initialize()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            var controllerTypes = assemblies
                .SelectMany(assembly => GetAvailableTypes(assembly)
                    .Where(type => type
                        .IsSubclassOf(typeof(Controller))));

            foreach (var controllerType in controllerTypes)
            {
                var methods = controllerType
                    .GetMethods()
                    .Where(method => method.CustomAttributes
                        .Any(attribute => attribute.AttributeType == typeof(PageTypeRoutingAttribute)));

                foreach (var method in methods)
                {
                    var pageTypeClassNames = method.GetCustomAttribute<PageTypeRoutingAttribute>().PageTypeClassName;

                    foreach (var pageTypeClassName in pageTypeClassNames)
                    {
                        RoutingDictionary.Add(pageTypeClassName, method);
                    }
                }
            }
        }

        private static Type[] GetAvailableTypes(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (Exception)
            {
                return new Type[0];
            }
        }

        public static MethodInfo GetTargetControllerMethod(string className)
        {
            RoutingDictionary.TryGetValue(className, out var result);

            return result;
        }
    }
}
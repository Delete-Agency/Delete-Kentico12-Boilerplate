using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using DeleteBoilerplate.DynamicRouting.Attributes;

namespace DeleteBoilerplate.DynamicRouting.Config
{
    public static class PageTypeRoutingConfig
    {
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private static Dictionary<string, MethodInfo> _routingDictionary = new Dictionary<string, MethodInfo>();

        public static void Initialize()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(assembly => assembly.CustomAttributes
                    .Any(attribute => attribute.AttributeType == typeof(PageTypeRoutingAssemblyAttribute)));

            var controllerTypes = assemblies
                .SelectMany(assembly => assembly
                    .GetTypes()
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
                        _routingDictionary.Add(pageTypeClassName, method);
                    }
                }
            }
        }

        public static MethodInfo GetTargetControllerMethod(string className)
        {
            _routingDictionary.TryGetValue(className, out var result);

            return result;
        }
    }
}
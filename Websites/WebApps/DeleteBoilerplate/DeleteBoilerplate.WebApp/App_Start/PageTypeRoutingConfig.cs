using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using DeleteBoilerplate.DynamicRouting.Attributes;

namespace DeleteBoilerplate.WebApp
{
    public static class PageTypeRoutingConfig
    {
        public static Dictionary<string, MethodInfo> RoutingDictionary = new Dictionary<string, MethodInfo>();

        public static void CollectRoutingDefinitions()
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
                        RoutingDictionary.Add(pageTypeClassName, method);
                    }
                }
            }
        }
    }
}
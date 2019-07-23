using System;
using System.Linq;
using System.Reflection;
using LightInject;

namespace DeleteBoilerplate.WebApp
{
    public class DIConfig
    {
        public static void Bootstrap()
        {
            var container = new ServiceContainer();

            var assemblies = GetAssemblies();
            foreach (var assembly in assemblies)
            {
                container.RegisterAssembly(assembly);
            }
            container.RegisterControllers(assemblies);

            container.EnableMvc();
            container.EnableAnnotatedPropertyInjection();
        }

        private static Assembly[] GetAssemblies() => AppDomain.CurrentDomain.GetAssemblies().Where(x => 
                x.CustomAttributes.Any(ca => ca.AttributeType == typeof(CompositionRootTypeAttribute))).ToArray();

    }
}
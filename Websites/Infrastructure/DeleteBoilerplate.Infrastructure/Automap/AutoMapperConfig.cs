using System.Linq;
using System.Reflection;
using AutoMapper;
using DeleteBoilerplate.Infrastructure.Helpers;
using LightInject;

namespace DeleteBoilerplate.Infrastructure
{
    public static class AutoMapperConfig
    {
        public static MapperConfiguration BuildMapperConfiguration(ServiceContainer serviceContainer)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.ConstructServicesUsing(serviceContainer.GetInstance);
                var assemblies = AssemblyHelper.GetDiscoverableAsseblyAssemblies();
                foreach (var assembly in assemblies)
                {
                    var definedTypes = assembly.DefinedTypes;
                    var profiles = definedTypes.Where(type =>
                        typeof(Profile).GetTypeInfo().IsAssignableFrom(type) && !type.IsAbstract).ToArray();
                    if (profiles.Length == 0) continue;

                    foreach (var profile in profiles.Select(t => t.AsType()))
                    {
                        cfg.AddProfile(profile);
                    }
                }
            });
            config.AssertConfigurationIsValid();
            return config;
        }
    }
}
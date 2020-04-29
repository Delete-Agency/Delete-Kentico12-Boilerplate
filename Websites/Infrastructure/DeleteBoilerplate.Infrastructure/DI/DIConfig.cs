using System.Web.Configuration;
using AutoMapper;
using DeleteBoilerplate.Common;
using DeleteBoilerplate.Common.Helpers;
using DeleteBoilerplate.Infrastructure.Services;
using LightInject;
using LightInject.ServiceLocation;

namespace DeleteBoilerplate.Infrastructure
{
    public class DIConfig
    {
        public static LightInjectServiceLocator DefaultServiceLocator;

        public static ServiceContainer Container;

        public static void Bootstrap()
        {
                var container = new ServiceContainer();

                var assemblies = AssemblyHelper.GetDiscoverableAssemblyAssemblies();
                foreach (var assembly in assemblies)
                {
                    container.RegisterAssembly(assembly);
                }
                container.RegisterControllers(assemblies);

                container.EnableMvc();
                container.EnableAnnotatedPropertyInjection();

                container.RegisterInstance<MapperConfiguration>(AutoMapperConfig.BuildMapperConfiguration(container));
                container.Register<IMapper>(c => new Mapper(c.GetInstance<MapperConfiguration>(), c.GetInstance));
                container.Register<IHashService>(c => new HashService(WebConfigurationManager.AppSettings["CMSHashStringSalt"]));

                DefaultServiceLocator = new LightInjectServiceLocator(container);
                Container = container;
        }
    }
}
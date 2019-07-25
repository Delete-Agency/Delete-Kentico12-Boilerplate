using DeleteBoilerplate.Infrastructure.Helpers;
using LightInject;

namespace DeleteBoilerplate.Infrastructure
{
    public class DIConfig
    {
        public static void Bootstrap()
        {
            var container = new ServiceContainer();

            var assemblies = AssemblyHelper.GetDiscoverableAsseblyAssemblies();
            foreach (var assembly in assemblies)
            {
                container.RegisterAssembly(assembly);
            }
            container.RegisterControllers(assemblies);

            container.EnableMvc();
            container.EnableAnnotatedPropertyInjection();

            container.RegisterInstance<MapperConfiguration>(AutoMapperConfig.BuildMapperConfiguration(container));
            container.Register<IMapper>(c => new Mapper(c.GetInstance<MapperConfiguration>(),c.GetInstance));
        }

    }
}
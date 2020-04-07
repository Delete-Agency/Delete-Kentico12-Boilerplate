using System.Linq;
using DeleteBoilerplate.Domain.Repositories;
using DeleteBoilerplate.Domain.RepositoryCaching;
using DeleteBoilerplate.Domain.RepositoryCaching.Keys;
using DeleteBoilerplate.Domain.RepositoryCaching.Providers;
using DeleteBoilerplate.Domain.Services;
using LightInject;

namespace DeleteBoilerplate.Domain
{
    public class CompositionRoot : ICompositionRoot
    {
        public void Compose(IServiceRegistry serviceRegistry)
        {
            serviceRegistry.RegisterScoped<IProjectRepository, ProjectRepository>();
            serviceRegistry.RegisterScoped<ITaxonomyRepository, TaxonomyRepository>();
            serviceRegistry.Register(typeof(ITaxonomySearch<>),typeof(TaxonomySearch<>), new PerScopeLifetime());
            serviceRegistry.RegisterScoped<ITaxonomyService, TaxonomyService>();
            serviceRegistry.RegisterScoped<ISocialLinksRepository, SocialLinksRepository>();
            serviceRegistry.RegisterScoped<INavigationRepository, NavigationRepository>();
            serviceRegistry.RegisterScoped<IStaticHtmlChunkRepository, StaticHtmlChunkRepository>();
           
            serviceRegistry.RegisterSingleton<IContentItemMetadataProvider, ContentItemMetadataProvider>();
            serviceRegistry.RegisterSingleton<IDependencyCacheKey, DependencyCacheKey>();
            serviceRegistry.RegisterScoped<ICachingRepositoryInterceptor, CachingRepositoryInterceptor>();
            RegisterRepositories(serviceRegistry);

            serviceRegistry.RegisterSingleton<IMailService, MailService>();
        }

        private static void RegisterRepositories(IServiceRegistry serviceRegistry)
        {
            var domainTypes = typeof(IRepository<>).Assembly.GetTypes();

            foreach (var domainType in domainTypes)
            {
                if (!domainType.IsClass && domainType.IsAbstract)
                    continue;

                var interfaces = domainType.GetInterfaces();

                var inheritedFromIRepository = interfaces
                    .Where(x => x.GetInterfaces().Any(y => y.IsGenericType && typeof(IRepository<>).IsAssignableFrom(y.GetGenericTypeDefinition()))
                                || x.IsGenericType && typeof(IRepository<>).IsAssignableFrom(x.GetGenericTypeDefinition()));

                foreach (var @interface in inheritedFromIRepository)
                {
                    serviceRegistry.Register(@interface, domainType, new PerContainerLifetime())
                        .Intercept(serviceRegistration => serviceRegistration.ServiceType == @interface, factory => factory.GetInstance<ICachingRepositoryInterceptor>());
                }
            }
        }

    }
}
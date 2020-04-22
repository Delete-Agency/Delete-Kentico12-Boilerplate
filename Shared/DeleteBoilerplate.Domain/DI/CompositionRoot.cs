using System;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using CMS.Core;
using CMS.Helpers;
using CMS.Personas;
using DeleteBoilerplate.Domain.Repositories;
using DeleteBoilerplate.Domain.RepositoryCaching;
using DeleteBoilerplate.Domain.RepositoryCaching.Keys;
using DeleteBoilerplate.Domain.RepositoryCaching.Providers;
using DeleteBoilerplate.Domain.Services;
using Kentico.Forms.Web.Mvc;
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

            serviceRegistry.RegisterSingleton<IUrlSelectorService, UrlSelectorService>();

            serviceRegistry.RegisterSingleton<IPersonaPictureUrlCreator, PersonaPictureUrlCreator>();
            
            serviceRegistry.RegisterSingleton<IContentItemMetadataProvider, ContentItemMetadataProvider>();
            serviceRegistry.RegisterSingleton<IDependencyCacheKey, DependencyCacheKey>();
            serviceRegistry.RegisterScoped<ICachingRepositoryInterceptor, CachingRepositoryInterceptor>();
            RegisterRepositories(serviceRegistry);

            serviceRegistry.Register(factory => Service.Resolve<ICurrentCookieLevelProvider>());

            serviceRegistry.RegisterSingleton<IMailService, MailService>();

            serviceRegistry.RegisterSingleton<IFormProvider, FormProvider>();
            serviceRegistry.RegisterSingleton<IFormComponentVisibilityEvaluator, FormComponentVisibilityEvaluator>();
            serviceRegistry.Register(factory => Service.Resolve<IFormComponentModelBinder>());
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
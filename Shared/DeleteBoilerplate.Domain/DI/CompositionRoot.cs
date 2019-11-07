using DeleteBoilerplate.Domain.Repositories;
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
        }
    }
}
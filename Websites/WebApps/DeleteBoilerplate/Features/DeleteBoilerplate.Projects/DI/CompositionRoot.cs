using DeleteBoilerplate.Domain.Repositories;
using DeleteBoilerplate.Projects.Services;
using LightInject;

namespace DeleteBoilerplate.Projects.DI
{
    public class CompositionRoot : ICompositionRoot
    {
        public void Compose(IServiceRegistry serviceRegistry)
        {
            serviceRegistry.RegisterScoped<IProjectRepository, ProjectRepository>();
            serviceRegistry.RegisterScoped<IProjectDescriber, ProjectDescriber>();
        }
    }
}
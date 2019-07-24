using DeleteBoilerplate.Domain.Repositories;
using LightInject;

namespace DeleteBoilerplate.Domain.DI
{
    public class CompositionRoot : ICompositionRoot
    {
        public void Compose(IServiceRegistry serviceRegistry)
        {
            serviceRegistry.RegisterScoped<IProjectRepository, ProjectRepository>();
        }
    }
}

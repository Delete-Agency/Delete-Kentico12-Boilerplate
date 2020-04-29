using DeleteBoilerplate.Twitter.Factory;
using DeleteBoilerplate.Twitter.Services;
using LightInject;

namespace DeleteBoilerplate.Twitter.DI
{
    public class CompositionRoot : ICompositionRoot
    {
        public void Compose(IServiceRegistry serviceRegistry)
        {
            serviceRegistry.Register(factory => TwitterContextFactory.CreateTwitterContext(), new PerContainerLifetime());
            serviceRegistry.RegisterSingleton<ITwitterService, TwitterService>();
        }
    }
}
using LightInject;

namespace DeleteBoilerplate.OutputCache.DI
{
    public class CompositionRoot : ICompositionRoot
    {
        public void Compose(IServiceRegistry serviceRegistry)
        {
            serviceRegistry.RegisterScoped<IOutputCacheDependencies,OutputCacheDependencies>();
        }
    }
}
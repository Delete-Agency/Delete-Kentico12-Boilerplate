using DeleteBoilerplate.DynamicRouting.Contexts;
using DeleteBoilerplate.DynamicRouting.Contexts.Implementation;
using LightInject;

namespace DeleteBoilerplate.DynamicRouting.DI
{
    public class CompositionRoot : ICompositionRoot
    {
        public void Compose(IServiceRegistry serviceRegistry)
        {
            serviceRegistry.RegisterScoped<IRequestContext, RequestContext>();
        }
    }
}
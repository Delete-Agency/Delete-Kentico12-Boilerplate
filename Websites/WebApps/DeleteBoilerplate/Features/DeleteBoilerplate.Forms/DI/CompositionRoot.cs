using CMS.Core;
using DeleteBoilerplate.Forms.Services;
using Kentico.Forms.Web.Mvc;
using LightInject;

namespace DeleteBoilerplate.Forms.DI
{
    public class CompositionRoot : ICompositionRoot
    {
        public void Compose(IServiceRegistry serviceRegistry)
        {
            serviceRegistry.Register(factory => Service.Resolve<IFormProvider>());
            serviceRegistry.Register(factory => Service.Resolve<IFormComponentVisibilityEvaluator>());
            serviceRegistry.Register(factory => Service.Resolve<IFormComponentModelBinder>());

            serviceRegistry.Register<ICaptchaVerificationService, CaptchaVerificationService>(new PerRequestLifeTime());
        }
    }
}
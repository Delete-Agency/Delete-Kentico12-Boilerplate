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
            serviceRegistry.Register<ICaptchaVerificationService, CaptchaVerificationService>(new PerRequestLifeTime());
        }
    }
}
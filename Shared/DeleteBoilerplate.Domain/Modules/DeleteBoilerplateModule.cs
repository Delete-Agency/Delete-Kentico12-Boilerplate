using CMS;
using CMS.DataEngine;
using CMS.Helpers;
using DeleteBoilerplate.Domain.Modules;
using DeleteBoilerplate.Domain.WebFarmTasks;

[assembly: RegisterModule(typeof(DeleteBoilerplateModule))]
namespace DeleteBoilerplate.Domain.Modules
{
    public class DeleteBoilerplateModule : Module
    {
        public DeleteBoilerplateModule()
            : base("DeleteBoilerplateModule")
        {
        }

        protected override void OnInit()
        {
            base.OnInit();

            WebFarmHelper.RegisterTask<CustomFileSyncWebFarmTask>();
        }
    }
}

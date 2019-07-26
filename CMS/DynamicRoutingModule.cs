using CMS.DataEngine;
using CMS.DocumentEngine;

namespace DeleteBoilerplate.Modules
{
    public class DynamicRoutingModule : Module
    {
        public DynamicRoutingModule() : base("DynamicRouting")
        {
        }

        protected override void OnInit()
        {
            base.OnInit();

            DocumentEvents.Update.Before += UpdateOnBefore;
            DocumentEvents.Insert.Before += UpdateOnBefore;
        }

        private void UpdateOnBefore(object sender, DocumentEventArgs e)
        {
            var basePageType = DataClassInfoProvider.GetClasses()
                .Where(dataClass => string.Equals(dataClass.Name, BasePage.CLASS_NAME))

            throw new System.NotImplementedException();
        }
    }
}

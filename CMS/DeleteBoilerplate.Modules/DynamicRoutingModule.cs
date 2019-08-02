using CMS;
using CMS.DataEngine;
using CMS.DocumentEngine;
using DeleteBoilerplate.Modules;

[assembly: RegisterModule(typeof(DynamicRoutingModule))]

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

            DocumentEvents.Update.Before += UpdateSeoUrlOnInsertOrUpdate;
            DocumentEvents.Insert.After += UpdateSeoUrlOnInsertOrUpdate;
        }

        private void UpdateSeoUrlOnInsertOrUpdate(object sender, DocumentEventArgs e)
        {
            if (e.Node.ContainsColumn("SeoUrl"))
                this.EnsurePopulatedSeoField(e.Node);
        }

        private void EnsurePopulatedSeoField(TreeNode node)
        {
            var seoUrl = (string)node["SeoUrl"];
            var updated = false;

            if (string.IsNullOrWhiteSpace(seoUrl))
            {
                node["SeoUrl"] = node.NodeAliasPath;
                updated = true;
            }
            else if (!seoUrl.StartsWith("/"))
            {
                node["SeoUrl"] = $"/{node["SeoUrl"]}";
                updated = true;
            }

            if (updated)
                node.Update();
        }
    }
}

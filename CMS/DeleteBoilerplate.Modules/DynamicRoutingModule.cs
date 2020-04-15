using System;
using System.Linq;
using CMS;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Helpers;
using DeleteBoilerplate.Domain;
using DeleteBoilerplate.Domain.Services;
using DeleteBoilerplate.Modules;

[assembly: RegisterModule(typeof(DynamicRoutingModule))]

namespace DeleteBoilerplate.Modules
{
    public class DynamicRoutingModule : Module
    {
        protected ISeoUrlService SeoUrlService = new SeoUrlService();

        public DynamicRoutingModule() : base("DeleteBoilerplate.Modules.DynamicRouting")
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
            if (e.Node.ContainsColumn(Constants.DynamicRouting.SeoUrlFieldName))
            {
                EnsureSeoUrlPopulated(e);
                EnsureSeoUrlUnique(e);
            }
        }

        private void EnsureSeoUrlPopulated(DocumentEventArgs eventArgs)
        {
            var node = eventArgs.Node;

            var seoUrl = ValidationHelper.GetString(node[Constants.DynamicRouting.SeoUrlFieldName], String.Empty);
            var updated = false;

            if (string.IsNullOrWhiteSpace(seoUrl))
            {
                node[Constants.DynamicRouting.SeoUrlFieldName] = node.NodeAliasPath;
                updated = true;
            }
            else if (!seoUrl.StartsWith("/"))
            {
                node[Constants.DynamicRouting.SeoUrlFieldName] = $"/{node[Constants.DynamicRouting.SeoUrlFieldName]}";
                updated = true;
            }

            if (updated)
                node.Update();
        }

        private void EnsureSeoUrlUnique(DocumentEventArgs eventArgs)
        {
            var seoUrl = ValidationHelper.GetString(eventArgs.Node[Constants.DynamicRouting.SeoUrlFieldName], String.Empty);

            var foundNodes = SeoUrlService.GetAllDocumentsBySeoUrl(seoUrl);

            if (foundNodes.Count == 0 || foundNodes.All(x => x.DocumentID == eventArgs.Node.DocumentID)) return;

            eventArgs.Cancel();
            throw new Exception(
                $"URL '{seoUrl}' is in conflict with another URL used for the page '{foundNodes.FirstOrDefault()?.DocumentNamePath ?? String.Empty}' ({foundNodes.FirstOrDefault()?.DocumentCulture ?? String.Empty}) page.");
        }
    }
}

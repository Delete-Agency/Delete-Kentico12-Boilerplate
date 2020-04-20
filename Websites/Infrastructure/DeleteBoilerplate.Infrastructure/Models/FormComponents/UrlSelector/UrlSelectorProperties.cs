using DeleteBoilerplate.Common.Models;
using Kentico.Forms.Web.Mvc;

namespace DeleteBoilerplate.Infrastructure.Models.FormComponents.UrlSelector
{
    public class UrlSelectorProperties : FormComponentProperties<UrlSelectorItem>
    {
        public UrlSelectorProperties() : base("", -1, -1)
        {
        }

        [DefaultValueEditingComponent(UrlSelectorComponent.Identifier)]
        public override UrlSelectorItem DefaultValue { get; set; }

        public string RootPath { get; set; }
    }
}
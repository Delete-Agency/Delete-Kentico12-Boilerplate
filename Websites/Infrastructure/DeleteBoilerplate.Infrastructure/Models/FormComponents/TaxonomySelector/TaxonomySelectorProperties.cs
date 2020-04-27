using CMS.DataEngine;
using Kentico.Forms.Web.Mvc;

namespace DeleteBoilerplate.Infrastructure.Models.FormComponents.TaxonomySelector
{
    public class TaxonomySelectorProperties : FormComponentProperties<string>
    {
        public TaxonomySelectorProperties() : base(FieldDataType.Text)
        {
        }

        [DefaultValueEditingComponent(TaxonomySelectorComponent.Identifier)]
        public override string DefaultValue { get; set; }

        public string TargetTaxonomyTypes { get; set; }
    }
}
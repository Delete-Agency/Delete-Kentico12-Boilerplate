using CMS.DataEngine;
using Kentico.Forms.Web.Mvc;

namespace DeleteBoilerplate.GenericComponents.Models.FormComponents
{
    public class TaxonomySelectorProperties : FormComponentProperties<string>
    {
        public TaxonomySelectorProperties() : base(FieldDataType.Text)
        {
        }

        [DefaultValueEditingComponent(FormComponentsIdentifiers.TaxonomySelector)]
        public override string DefaultValue { get; set; }

        public string TargetTaxonomyTypes { get; set; }
    }
}
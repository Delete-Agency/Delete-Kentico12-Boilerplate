using DeleteBoilerplate.GenericComponents.Models.FormComponents;
using Kentico.Forms.Web.Mvc;

[assembly:
    RegisterFormComponent(FormComponentsIdentifiers.TaxonomySelector, typeof(TaxonomySelectorComponent), "Taxonomy selector",
        IsAvailableInFormBuilderEditor = false, ViewName = "FormComponents/_TaxonomySelectorComponent")]

namespace DeleteBoilerplate.GenericComponents.Models.FormComponents
{
    public class TaxonomySelectorComponent : FormComponent<TaxonomySelectorProperties, string>
    {
        [BindableProperty]
        public string SelectedTaxonomies { get; set; }

        public string TargetTaxonomyTypes { get; set; }

        public override string GetValue()
        {
            return SelectedTaxonomies;
        }

        public override void SetValue(string value)
        {
            SelectedTaxonomies = value;
        }

        public override void LoadProperties(FormComponentProperties properties)
        {
            base.LoadProperties(properties);
            TargetTaxonomyTypes = (properties as TaxonomySelectorProperties)?.TargetTaxonomyTypes;
        }
    }
}
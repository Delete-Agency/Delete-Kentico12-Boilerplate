using System.Collections.Generic;
using CMS.DocumentEngine.Types.DeleteBoilerplate;
using DeleteBoilerplate.GenericComponents.Models.FormComponents;
using DeleteBoilerplate.GenericComponents.Models.Widgets;
using Kentico.Components.Web.Mvc.FormComponents;
using Kentico.Forms.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc;

namespace DeleteBoilerplate.Projects.Models.Widgets.ProjectsListing
{
    public class ProjectsListingWidgetProperties : IListingWidgetProperties<Project>, IWidgetProperties, ITaxonomyWidgetProperties
    {
        [EditingComponent(PathSelector.IDENTIFIER)]
        [EditingComponentProperty(nameof(PathSelectorProperties.RootPath), "/")]
        [EditingComponentProperty(nameof(PathSelectorProperties.Label), "Projects Container Page")]
        public List<PathSelectorItem> Items { get; set; }

        [EditingComponent(FormComponentsIdentifiers.TaxonomySelector, Order = 0, Label = "Area")]
        [EditingComponentProperty("TargetTaxonomyType", "Area")]
        public string Type { get; set; }
    }
}
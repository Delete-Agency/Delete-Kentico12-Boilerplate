using System.Collections.Generic;
using CMS.DocumentEngine.Types.DeleteBoilerplate;
using DeleteBoilerplate.Common.Models;
using DeleteBoilerplate.GenericComponents.Models.FormComponents;
using DeleteBoilerplate.GenericComponents.Models.Widgets;
using DeleteBoilerplate.Infrastructure.Models;
using DeleteBoilerplate.Infrastructure.Models.FormComponents;
using Kentico.Components.Web.Mvc.FormComponents;
using Kentico.Forms.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc;

namespace DeleteBoilerplate.Projects.Models.Widgets.ProjectsListing
{
    public class ProjectsListingWidgetProperties : BaseWidgetViewModel, IListingWidgetProperties<Project>, IWidgetProperties, ITaxonomyWidgetProperties
    {
        [EditingComponent(PathSelector.IDENTIFIER)]
        [EditingComponentProperty(nameof(PathSelectorProperties.RootPath), "/")]
        [EditingComponentProperty(nameof(PathSelectorProperties.Label), "Projects Container Page")]
        public List<PathSelectorItem> Items { get; set; }

        [EditingComponent(FormComponentsIdentifiers.TaxonomySelector, Order = 0, Label = "Area")]
        [EditingComponentProperty("TargetTaxonomyTypes", "Area;Country")]
        public string Type { get; set; }

        [EditingComponent(UrlSelectorComponent.Identifier, Order = 10)]
        public UrlSelectorItem Link { get; set; }
    }
}
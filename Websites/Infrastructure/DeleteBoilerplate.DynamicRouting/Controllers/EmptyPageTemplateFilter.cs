using System.Collections.Generic;
using System.Linq;
using Kentico.PageBuilder.Web.Mvc.PageTemplates;

namespace DeleteBoilerplate.DynamicRouting.Controllers
{
    /// <summary>
    /// This is to prevent a template from automatically being assigned. If there is 1 non empty template that is available, this will add the "Empty" template as an option so the user can select.
    /// </summary>
    public class EmptyPageTemplateFilter : IPageTemplateFilter
    {
        public IEnumerable<PageTemplateDefinition> Filter(IEnumerable<PageTemplateDefinition> pageTemplates, PageTemplateFilterContext context)
        {
            // only add empty option if there is 1 non empty template remaining, so user has to choose.
            var pageTemplateDefinitions = pageTemplates.ToList();

            var nonEmptyTemplates = pageTemplateDefinitions.Where(t => !GetTemplates().Contains(t.Identifier));

            if (nonEmptyTemplates.Any())
            {
                return pageTemplateDefinitions;
            }

            // Remove the empty template as an option
            return pageTemplateDefinitions.Where(t => !GetTemplates().Contains(t.Identifier));
        }

        // Gets all page templates that are allowed for landing pages
        public IEnumerable<string> GetTemplates() => new string[] { "Empty.Template" };
    }
}
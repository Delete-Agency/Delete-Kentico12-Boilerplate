using System.Web.Mvc;
using DeleteBoilerplate.GenericComponents.Controllers.Sections;
using Kentico.PageBuilder.Web.Mvc;

[assembly: RegisterSection("DeleteBoilerplate.OneFullWidthColumnSection", typeof(OneFullWidthColumnSectionController),
    "{$DeleteBoilerplate.Section.OneFullWidthColumn.Name$}", Description = "{$DeleteBoilerplate.Section.OneFullWidthColumn.Description$}", IconClass = "icon-l-square")]

namespace DeleteBoilerplate.GenericComponents.Controllers.Sections
{
    public class OneFullWidthColumnSectionController : Controller
    {
        public ActionResult Index()
        {
            return PartialView("Sections/_OneFullWidthColumnSection");
        }
    }
}
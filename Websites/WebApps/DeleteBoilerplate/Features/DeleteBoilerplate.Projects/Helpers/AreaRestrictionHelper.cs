using System;
using System.Linq;

namespace DeleteBoilerplate.Projects.Helpers
{
    public static class AreaRestrictionHelper
    {
        public static string[] GetProjectsMainRestrictions()
        {
            return Infrastructure.Helpers.AreaRestrictionHelper.GetWidgetsIdentifiers()
                .Where(x => x.StartsWith("DeleteBoilerplate.GenericComponents.", StringComparison.OrdinalIgnoreCase) ||
                            x.StartsWith("DeleteBoilerplate.Projects.", StringComparison.OrdinalIgnoreCase))
                .ToArray();
        }
    }
}

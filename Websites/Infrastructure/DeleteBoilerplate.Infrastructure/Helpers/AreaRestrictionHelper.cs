using System.Collections.Generic;
using System.Linq;
using Kentico.PageBuilder.Web.Mvc;

namespace DeleteBoilerplate.Infrastructure.Helpers
{
    public static class AreaRestrictionHelper
    {
        private static IEnumerable<string> _widgetIdentifiers;

        public static IEnumerable<string> GetWidgetsIdentifiers()
        {
#if !_NOCACHE
            if (_widgetIdentifiers == null)
#endif
            {
                _widgetIdentifiers = new ComponentDefinitionProvider<WidgetDefinition>()
                    .GetAll()
                    .Select(definition => definition.Identifier);
            }

            return _widgetIdentifiers;
        }
    }
}

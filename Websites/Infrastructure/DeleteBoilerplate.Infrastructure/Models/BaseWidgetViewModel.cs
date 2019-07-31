using CMS.Helpers;

namespace DeleteBoilerplate.Infrastructure.Models
{
    public abstract class BaseWidgetViewModel
    {
        public string GetPlaceholder(string propertyName)
        {
            return ResHelper.GetString($"DeleteBoilerplate.Placeholder.{propertyName}");
        }
    }
}

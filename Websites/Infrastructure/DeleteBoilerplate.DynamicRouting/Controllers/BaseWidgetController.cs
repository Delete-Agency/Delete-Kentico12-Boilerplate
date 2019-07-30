using Kentico.PageBuilder.Web.Mvc;

namespace DeleteBoilerplate.DynamicRouting.Controllers
{
    public abstract class BaseWidgetController<TPropertiesType> : WidgetController<TPropertiesType> where TPropertiesType : class, IWidgetProperties, new()
    {
    }
}
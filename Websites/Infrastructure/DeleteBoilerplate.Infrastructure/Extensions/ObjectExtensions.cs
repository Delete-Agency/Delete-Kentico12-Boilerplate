using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using DeleteBoilerplate.Infrastructure.Helpers;

namespace DeleteBoilerplate.Infrastructure.Extensions
{
    public static class ObjectExtensions
    {
        public static NameValueCollection GetHtmlAttributeCollection(this object target, bool lowerCaseName = false, bool underscoreForHyphens = true)
        {
            var nameValueCollection = new NameValueCollection();
            if (target == null) return nameValueCollection;

            foreach (var property in PropertyInfoHelper.GetAllProperties(target.GetType()).OrderBy(p => p.Name == "class"))
            {
                var propertyName = lowerCaseName ? property.Name.ToLower() : property.Name;
                if (underscoreForHyphens)
                {
                    propertyName = propertyName.Replace("_", "-");
                }

                var propertyValue = property.GetValue(target);
                if (propertyValue == null)
                {
                    nameValueCollection.Add(propertyName, string.Empty);
                    continue;
                }

                var val = propertyValue.GetType().IsArray
                    ? string.Join(",", propertyValue as IEnumerable)
                    : propertyValue.ToString();

                nameValueCollection.Add(propertyName, val);
            }

            return nameValueCollection;
        }
    }
}
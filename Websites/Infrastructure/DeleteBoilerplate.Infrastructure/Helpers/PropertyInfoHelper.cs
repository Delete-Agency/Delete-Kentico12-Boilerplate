using System;
using System.Collections.Generic;
using System.Reflection;

namespace DeleteBoilerplate.Infrastructure.Helpers
{
    public static class PropertyInfoHelper
    {
        public static BindingFlags Flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;

        public static PropertyInfo GetProperty(Type type, string name)
        {
            PropertyInfo propertyInfo = null;
            try
            {
                propertyInfo = type.GetProperty(name, Flags);
            }
            catch (AmbiguousMatchException)
            {
            }
            if (propertyInfo == null)
            {
                foreach (var type1 in type.GetInterfaces())
                {
                    try
                    {
                        propertyInfo = type1.GetProperty(name);
                        if (propertyInfo != null)
                            return propertyInfo;
                    }
                    catch (AmbiguousMatchException)
                    {
                    }
                }
            }
            return propertyInfo;
        }

        public static IEnumerable<PropertyInfo> GetAllProperties(Type type)
        {
            var typeList = new List<Type> { type };
            if (type.IsInterface)
                typeList.AddRange(type.GetInterfaces());
            var propertyInfoList = new List<PropertyInfo>();
            foreach (var type1 in typeList)
            {
                foreach (var property1 in type1.GetProperties(Flags))
                {
                    var property2 = GetProperty(property1.DeclaringType, property1.Name);
                    propertyInfoList.Add(property2);
                }
            }
            return propertyInfoList;
        }
    }
}
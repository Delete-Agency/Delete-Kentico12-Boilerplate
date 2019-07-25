using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeleteBoilerplate.DynamicRouting
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class CustomRoutesAttribute : Attribute
    {
    }
}
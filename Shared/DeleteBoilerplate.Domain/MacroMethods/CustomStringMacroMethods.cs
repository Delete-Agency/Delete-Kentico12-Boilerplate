using CMS;
using CMS.Helpers;
using CMS.MacroEngine;
using DeleteBoilerplate.Common.Extensions;
using DeleteBoilerplate.Domain.MacroMethods;
using System;

// Makes all methods in the 'CustomMacroMethods' container class available for string objects
[assembly: RegisterExtension(typeof(CustomStringMacroMethods), typeof(string))]
// Registers methods from the 'CustomMacroMethods' container into the "String" macro namespace
[assembly: RegisterExtension(typeof(CustomStringMacroMethods), typeof(StringNamespace))]
namespace DeleteBoilerplate.Domain.MacroMethods
{
    public class CustomStringMacroMethods : MacroMethodContainer
    {
        [MacroMethod(typeof(string), "Resolve absolute url using relative and MVC site host", 1)]
        [MacroMethodParam(0, "relativeUrl", typeof(string), "")]
        public static object ResolveAbsoluteUrl(EvaluationContext context, params object[] parameters)
        {
            // Branches according to the number of the method's parameters
            switch (parameters.Length)
            {
                case 1:
                    // Overload with one parameter
                    return ValidationHelper.GetString((parameters[0] as string).GetAbsoluteUrl(), "");
                default:
                    // No other overloads are supported
                    throw new NotSupportedException();
            }
        }
    }
}
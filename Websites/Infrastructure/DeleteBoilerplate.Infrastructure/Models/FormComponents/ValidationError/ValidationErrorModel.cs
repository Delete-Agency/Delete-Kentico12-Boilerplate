using DeleteBoilerplate.Common.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace DeleteBoilerplate.Infrastructure.Models.FormComponents.ValidationError
{
    public class ValidationErrorModel
    {
        public Dictionary<string, string> Errors { get; set; }

        public static ValidationErrorModel Build(ModelStateDictionary modelState)
        {
            var result = new ValidationErrorModel
            {
                Errors = new Dictionary<string, string>()
            };

            foreach (var state in modelState)
            {
                var key = state.Key;
                var errors = string.Join("<br/>", state.Value.Errors.Select(x => x.ErrorMessage).ToList());

                if (!errors.IsNullOrWhiteSpace())
                    result.Errors.Add(key, errors);
            }

            return result;
        }
    }
}
using DeleteBoilerplate.Common.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web.Mvc;

namespace DeleteBoilerplate.Forms.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum FormResponseType
    {
        [EnumMember(Value = "success")]
        Success,
        [EnumMember(Value = "serverError")]
        ServerError,
        [EnumMember(Value = "validationErrors")]
        ValidationErrors
    }

    public class FormResponseModel
    {
        [JsonProperty(PropertyName = "type")]
        public FormResponseType Type { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        [JsonProperty(PropertyName = "errors")]
        public Dictionary<string, string> Errors { get; set; }

        public static FormResponseModel BuildValidationErrorResponse(ModelStateDictionary modelState)
        {
            var result = new FormResponseModel
            {
                Type = FormResponseType.ValidationErrors,
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
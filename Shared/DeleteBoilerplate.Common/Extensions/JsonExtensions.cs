using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;

namespace DeleteBoilerplate.Common.Extensions
{
    public static class JsonExtensions
    {
        public static JsonSerializerSettings DefaultJsonSerializerSettings => new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Include,
            DateFormatString = "yyyy-MM-ddTHH:mm:ss",
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        public static TObject DeserializeJson<TObject>(this string json)
        {
            return json.DeserializeJson<TObject>(DefaultJsonSerializerSettings);
        }

        public static TObject DeserializeJson<TObject>(this string json, JsonSerializerSettings jsonSerializerSettings)
        {
            return JsonConvert.DeserializeObject<TObject>(json, jsonSerializerSettings);
        }

        public static object DeserializeJson(this string json, Type objectType)
        {
            return json.DeserializeJson(objectType, DefaultJsonSerializerSettings);
        }

        public static object DeserializeJson(this string json, Type objectType, JsonSerializerSettings jsonSerializerSettings)
        {
            return JsonConvert.DeserializeObject(json, objectType, jsonSerializerSettings);
        }

    }
}

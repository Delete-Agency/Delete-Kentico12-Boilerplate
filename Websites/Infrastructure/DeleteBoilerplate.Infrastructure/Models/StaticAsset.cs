using System;
using Newtonsoft.Json;
using DeleteBoilerplate.Infrastructure.Extensions;

namespace DeleteBoilerplate.Infrastructure.Models
{
    [Serializable]
    public class StaticAsset
    {
        public string Name { get; set; }
        [JsonProperty(PropertyName = "path")]
        public string Path { get; set; }
        [JsonIgnore]
        public bool IsLocal => HtmlHelperExtensions.IsLocal(this.Path);
        public int Order = 50;
    }
}

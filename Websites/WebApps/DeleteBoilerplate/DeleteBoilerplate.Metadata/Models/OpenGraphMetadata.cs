using DeleteBoilerplate.Infrastructure.Enums;

namespace DeleteBoilerplate.Metadata.Models
{
    public class OpenGraphMetadata
    {
        public OpenGraphType Type { get; set; }

        public string Image { get; set; }

        public string ImageAlt { get; set; }

        public string SiteName { get; set; }

        public string Url { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }
    }
}
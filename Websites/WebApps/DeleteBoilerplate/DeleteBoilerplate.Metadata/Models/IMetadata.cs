namespace DeleteBoilerplate.Metadata.Models
{
    public interface IMetadata
    {
        string Title { get; set; }

        string Description { get; set; }

        string CanonicalUrl { get; set; }

        OpenGraphMetadata OpenGraphMetadata { get; set; }
    }
}

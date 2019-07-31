namespace DeleteBoilerplate.Domain.Models.PageTypes
{
    public interface IBasePage
    {
        string SeoUrl { get; set; }

        string Taxonomy { get; set; }
    }
}

using DeleteBoilerplate.Infrastructure.Attributes;

namespace DeleteBoilerplate.Infrastructure.Enums
{
    public enum OpenGraphType
    {
        [StringValue("article")]
        Article,
        [StringValue("news")]
        News,
        [StringValue("website")]
        Website,
        [StringValue("profile")]
        Profile
    }
}

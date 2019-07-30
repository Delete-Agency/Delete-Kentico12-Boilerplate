using DeleteBoilerplate.Infrastructure.Attributes;

namespace DeleteBoilerplate.Infrastructure.Enums
{
    public enum SiteSetting
    {
        Default,
        [StringValue("DeleteBoilerplate_DefaultImage")]
        DefaultImage,
        [StringValue("DeleteBoilerplate_DefaultImageAlt")]
        DefaultImageAlt
    }
}

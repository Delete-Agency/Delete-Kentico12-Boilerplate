using CMS.Base;

namespace CMS.DocumentEngine.Types.DeleteBoilerplate
{
    public interface IBasePage : ITreeNode
    {
        string SeoUrl { get; set; }

        string MetadataTitle { get; set; }

        string MetadataDescription { get; set; }
    }
}

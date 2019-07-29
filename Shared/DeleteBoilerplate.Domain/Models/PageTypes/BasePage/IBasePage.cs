using CMS.Base;

namespace CMS.DocumentEngine.Types.DeleteBoilerplate
{
    public interface IBasePage : ITreeNode
    {
        string SeoUrl { get; set; }
    }
}

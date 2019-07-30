using CMS.DocumentEngine;
using CMS.Helpers;

namespace DeleteBoilerplate.Infrastructure.Extensions
{
    public static class TreeNodeExtensions
    {
        public static string GetAbsoluteUrl(this TreeNode treeNode)
        {
            var absoluteUrl = URLHelper.ResolveUrl(treeNode.AbsoluteURL);

            return URLHelper.RemovePortFromURL(absoluteUrl);
        }
    }
}

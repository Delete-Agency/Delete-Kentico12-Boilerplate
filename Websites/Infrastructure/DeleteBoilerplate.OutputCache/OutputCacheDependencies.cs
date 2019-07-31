using System;
using System.Collections.Generic;
using System.Web;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.SiteProvider;

namespace DeleteBoilerplate.OutputCache
{
    public class OutputCacheDependencies : IOutputCacheDependencies
    {
        private readonly HashSet<string> _dependencyCacheKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        public void AddPageDependency<T>() where T : TreeNode, new()
        {
            AddCacheItemDependency($"nodes|{SiteContext.CurrentSiteName.ToLowerInvariant()}|{new T().ClassName}|all");
        }
        public void AddPageTypeDependency(string pageType)
        {
            AddCacheItemDependency($"nodes|{SiteContext.CurrentSiteName}|{pageType}");
        }

        public void AddDocumentIdDependency(int documentId)
        {
            AddCacheItemDependency($"documentid|{documentId}");
        }

        public void AddNodeIdDependency(int nodeId)
        {
            AddCacheItemDependency($"nodeid|{nodeId}");
        }

        public void AddNodeGuidDependency(Guid nodeGuid)
        {
            AddCacheItemDependency($"nodeguid|{SiteContext.CurrentSiteName}|{nodeGuid:D}");
        }

        public void AddDummyKeyDependency(string dummyKey)
        {
            AddCacheItemDependency(dummyKey);
        }

        private void AddCacheItemDependency(string dependencyCacheKey)
        {
            var lower = dependencyCacheKey.ToLowerInvariant();

            if (!_dependencyCacheKeys.Contains(lower))
            {
                _dependencyCacheKeys.Add(lower);
                CacheHelper.EnsureDummyKey(dependencyCacheKey);
                HttpContext.Current.Response.AddCacheItemDependency(lower);
            }
        }
    }
}
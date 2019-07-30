using System;
using System.Collections.Generic;
using System.Web;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.SiteProvider;
using LightInject;

namespace DeleteBoilerplate.OutputCache
{
    public class OutputCacheDependencies : IOutputCacheDependencies
    {
        private readonly HashSet<string> _dependencyCacheKeys = new HashSet<string>();

        [Inject]
        public HttpResponseBase Response { get; set; }

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
            if (!_dependencyCacheKeys.Contains(dependencyCacheKey))
            {
                _dependencyCacheKeys.Add(dependencyCacheKey);
                CacheHelper.EnsureDummyKey(dependencyCacheKey);
                Response.AddCacheItemDependency(dependencyCacheKey);
            }
        }
    }
}
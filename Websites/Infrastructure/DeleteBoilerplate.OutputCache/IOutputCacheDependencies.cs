using System;
using CMS.DocumentEngine;

namespace DeleteBoilerplate.OutputCache
{
    public interface IOutputCacheDependencies
    {
        void AddPageDependency<T>() where T : TreeNode, new();

        void AddPageTypeDependency(string pageType);

        void AddDocumentIdDependency(int documentId);

        void AddNodeIdDependency(int nodeId);

        void AddNodeGuidDependency(Guid nodeGuid);

        void AddDummyKeyDependency(string dummyKey);
    }
}

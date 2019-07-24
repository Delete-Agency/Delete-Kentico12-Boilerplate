using System.Collections.Generic;
using System.Data;
using System.Linq;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.SiteProvider;

namespace DeleteBoilerplate.DynamicRouting.Helpers
{
    /// <summary>
    /// DocumentQuery class with Caching taken into affect
    /// </summary>
    public class CacheableDocumentQuery : DocumentQuery
    {
        public string CacheItemName;
        public int CacheMinutes;
        public string[] CacheDependencies;
        /// <summary>
        /// Items that are used as part of the CacheItemName, and create a unique cache name.
        /// </summary>
        public List<object> CacheItemNameParts;
        /// <summary>
        /// Any additional Cache Dependencies, these are often added automatically for things like NodeID or DocumentID
        /// </summary>
        public List<string> CacheDependencyParts;
        public CacheableDocumentQuery() : base()
        {
            CacheItemNameParts = new List<object>();
            CacheDependencyParts = new List<string>();
        }
        public CacheableDocumentQuery(string className) : base(className)
        {
            CacheItemNameParts = new List<object>();
            CacheDependencyParts = new List<string>();
        }

        /// <summary>
        /// Gets the Results (DataSet) of the results, caching if applicable
        /// </summary>
        /// <returns>The DataSet of the results</returns>
        public DataSet GetCachedResult()
        {
            if (CacheMinutes == -1)
            {
                // Check cache settings
                CacheMinutes = CacheHelper.CacheMinutes(SiteContext.CurrentSiteName);
            }
            if (string.IsNullOrWhiteSpace(CacheItemName))
            {
                CacheItemName = string.Join("|", CacheItemNameParts);
            }
            if (!EnvironmentHelper.PreviewEnabled && CacheMinutes > 0 && !string.IsNullOrWhiteSpace(CacheItemName))
            {
                return CacheHelper.Cache<DataSet>(cs =>
                {
                    if (cs.Cached)
                    {
                        if (CacheDependencies != null)
                        {
                            CacheDependencyParts.AddRange(CacheDependencies);
                        }
                        cs.CacheDependency = CacheHelper.GetCacheDependency(CacheDependencyParts.Distinct().ToArray());
                    }
                    return Result;
                }, new CacheSettings(CacheMinutes, CacheItemName, "TreeNode_Result", CacheItemNameParts));
            }

            return Result;
        }

        /// <summary>
        /// Gets the Typed Results of the query with caching
        /// </summary>
        /// <returns>The Typed Results.</returns>
        public InfoDataSet<TreeNode> GetTypedResult()
        {
            if (CacheMinutes == -1)
            {
                // Check cache settings
                CacheMinutes = CacheHelper.CacheMinutes(SiteContext.CurrentSiteName);
            }
            if (string.IsNullOrWhiteSpace(CacheItemName))
            {
                CacheItemName = string.Join("|", CacheItemNameParts);
            }
            if (!EnvironmentHelper.PreviewEnabled && CacheMinutes > 0 && !string.IsNullOrWhiteSpace(CacheItemName))
            {
                return CacheHelper.Cache<InfoDataSet<TreeNode>>(cs =>
                {
                    if (cs.Cached)
                    {
                        if (CacheDependencies != null)
                        {
                            CacheDependencyParts.AddRange(CacheDependencies);
                        }
                        cs.CacheDependency = CacheHelper.GetCacheDependency(CacheDependencyParts.Distinct().ToArray());
                    }
                    return TypedResult;
                }, new CacheSettings(CacheMinutes, CacheItemName, "TreeNode_TypedResult", CacheItemNameParts));
            }

            return TypedResult;
        }

        /// <summary>
        /// Gets the Typed Results, populating the "Children" of the Nodes with their children.
        /// </summary>
        /// <returns>The TreeNodes (top level) with Children populated with their child elements.</returns>
        public InfoDataSet<TreeNode> GetHierarchicalTypedResult()
        {
            if (CacheMinutes == -1)
            {
                // Check cache settings
                CacheMinutes = CacheHelper.CacheMinutes(SiteContext.CurrentSiteName);
            }
            if (string.IsNullOrWhiteSpace(CacheItemName))
            {
                CacheItemName = string.Join("|", CacheItemNameParts);
            }
            if (!EnvironmentHelper.PreviewEnabled && CacheMinutes > 0 && !string.IsNullOrWhiteSpace(CacheItemName))
            {
                return CacheHelper.Cache<InfoDataSet<TreeNode>>(cs =>
                {
                    var parentNodeIdToTreeNode = new Dictionary<int, TreeNode>();
                    var compiledNodes = new List<TreeNode>();
                    if (cs.Cached)
                    {
                        if (CacheDependencies != null)
                        {
                            CacheDependencyParts.AddRange(CacheDependencies);
                        }
                        cs.CacheDependency = CacheHelper.GetCacheDependency(CacheDependencyParts.Distinct().ToArray());
                    }
                    // Populate the Children of the TypedResults
                    foreach (var node in TypedResult)
                    {
                        // Make sure Children only contain the children found in the typed results.
                        node.Children.MakeEmpty();
                        parentNodeIdToTreeNode.Add(node.NodeID, node);
                        // If no parent exists, add to top level
                        if (!parentNodeIdToTreeNode.ContainsKey(node.NodeParentID))
                        {
                            compiledNodes.Add(node);
                        }
                        else
                        {
                            // Otherwise, add to the parent element.
                            parentNodeIdToTreeNode[node.NodeParentID].Children.Add(node);
                        }
                    }
                    return new InfoDataSet<TreeNode>(compiledNodes.ToArray());
                }, new CacheSettings(CacheMinutes, CacheItemName, "TreeNode_TypedResult", CacheItemNameParts));
            }

            return TypedResult;
        }
    }
}
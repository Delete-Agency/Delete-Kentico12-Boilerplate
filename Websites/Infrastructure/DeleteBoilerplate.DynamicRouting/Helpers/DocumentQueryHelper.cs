using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.SiteProvider;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DeleteBoilerplate.DynamicRouting.Helpers
{
    public class DocumentQueryHelper
    {
        /// <summary>
        /// Gets the TreeNode for the corresponding path, can be either the NodeAliasPath or a URL Alias
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static TreeNode GetNodeByAliasPath(string path, string className = null, string cultureCode = null)
        {
            return CacheHelper.Cache(cs =>
            {
                var cacheDependencies = new List<string>();
                var foundNode = DocumentQueryHelper.RepeaterQuery(path: path, classNames: className, cultureCode: cultureCode).GetTypedResult().Items.FirstOrDefault();
                if (foundNode != null)
                {
                    cacheDependencies.Add("documentid|" + foundNode.DocumentID);
                }
                if (cs.Cached)
                {
                    cs.CacheDependency = CacheHelper.GetCacheDependency(cacheDependencies.ToArray());
                }
                return foundNode;
            }, new CacheSettings((EnvironmentHelper.PreviewEnabled ? 0 : CacheHelper.CacheMinutes(SiteContext.CurrentSiteName)), path, className, cultureCode, SiteContext.CurrentSiteName));
        }

        /// <summary>
        ///  DocumentQuery that mimics the Hierarchy Viewer webpart, with caching included.
        /// </summary>
        /// <param name="path">The Path for the documents to select</param>
        /// <param name="classNames">Class Names to include, semicolon seperated.</param>
        /// <param name="combineWithDefaultCulture"></param>
        /// <param name="cultureCode"></param>
        /// <param name="maxRelativeLevel"></param>
        /// <param name="useHierarchicalOrder">Ensures the NodeLevel, NodeOrder is set first as the order.</param>
        /// <param name="additionalOrderBy"></param>
        /// <param name="selectOnlyPublished"></param>
        /// <param name="siteName"></param>
        /// <param name="whereCondition"></param>
        /// <param name="columns"></param>
        /// <param name="filterOutDuplicates"></param>
        /// <param name="relationshipWithNodeGuid">The Relationship to use, defaults to the current page.</param>
        /// <param name="relatedNodeIsOnTheLeftSide"></param>
        /// <param name="relationshipName"></param>
        /// <param name="checkPermission"></param>
        /// <param name="loadPagesIndividually"></param>
        /// <param name="cacheItemName">The Cache Item Name, required if you wish to cache.</param>
        /// <param name="cacheMinutes">The Cache minutes, if not provided uses the site default.</param>
        /// <param name="cacheDependencies">Any cache dependencies</param>
        /// <returns></returns>
        public static CacheableDocumentQuery HierarchyViewerQuery(
            string path = "/%",
            string classNames = null,
            bool combineWithDefaultCulture = false,
            string cultureCode = null,
            int maxRelativeLevel = -1,
            bool useHierarchicalOrder = true,
            string additionalOrderBy = null,
            bool selectOnlyPublished = true,
            string siteName = null,
            string whereCondition = null,
            string columns = null,
            bool filterOutDuplicates = false,
            Guid relationshipWithNodeGuid = new Guid(),
            bool relatedNodeIsOnTheLeftSide = true,
            string relationshipName = null,
            bool checkPermission = false,
            bool loadPagesIndividually = false,
            string cacheItemName = null,
            int cacheMinutes = -1,
            string[] cacheDependencies = null
            )
        {
            additionalOrderBy = !string.IsNullOrWhiteSpace(additionalOrderBy) ? additionalOrderBy : "1";
            if (useHierarchicalOrder)
            {
                additionalOrderBy = SqlHelper.AddOrderBy(additionalOrderBy, "NodeLevel, NodeOrder");
            }
            return RepeaterQuery(
                path: path,
                classNames: classNames,
                combineWithDefaultCulture: combineWithDefaultCulture,
                cultureCode: cultureCode,
                maxRelativeLevel: maxRelativeLevel,
                orderBy: additionalOrderBy,
                selectOnlyPublished: selectOnlyPublished,
                selectTopN: -1,
                siteName: siteName,
                whereCondition: whereCondition,
                columns: columns,
                filterOutDuplicates: filterOutDuplicates,
                relationshipWithNodeGuid: relationshipWithNodeGuid,
                relatedNodeIsOnTheLeftSide: relatedNodeIsOnTheLeftSide,
                relationshipName: relationshipName,
                checkPermission: checkPermission,
                loadPagesIndividually: loadPagesIndividually,
                cacheItemName: cacheItemName,
                cacheMinutes: cacheMinutes,
                cacheDependencies: cacheDependencies);
        }

        /// <summary>
        /// DocumentQuery that mimics the Repeater Webpart, with caching included.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="nodeId">The NodeID</param>
        /// <param name="documentId">The DocumentID</param>
        /// <param name="classNames"></param>
        /// <param name="combineWithDefaultCulture"></param>
        /// <param name="cultureCode"></param>
        /// <param name="maxRelativeLevel"></param>
        /// <param name="orderBy"></param>
        /// <param name="selectOnlyPublished"></param>
        /// <param name="selectTopN"></param>
        /// <param name="siteName"></param>
        /// <param name="whereCondition"></param>
        /// <param name="columns"></param>
        /// <param name="filterOutDuplicates"></param>
        /// <param name="relationshipWithNodeGuid"></param>
        /// <param name="relatedNodeIsOnTheLeftSide"></param>
        /// <param name="relationshipName"></param>
        /// <param name="checkPermission"></param>
        /// <param name="loadPagesIndividually"></param>
        /// <param name="cacheItemName">The Cache Item Name, required if you wish to cache.</param>
        /// <param name="cacheMinutes">The Cache minutes, if not provided uses the site default.</param>
        /// <param name="cacheDependencies">Any cache dependencies</param>
        /// <returns></returns>
        public static CacheableDocumentQuery RepeaterQuery(
            string path = "/%",
            int nodeId = -1,
            int documentId = -1,
            string classNames = null,
            bool combineWithDefaultCulture = false,
            string cultureCode = null,
            int maxRelativeLevel = -1,
            string orderBy = null,
            bool selectOnlyPublished = true,
            int selectTopN = -1,
            string siteName = null,
            string whereCondition = null,
            string columns = null,
            bool filterOutDuplicates = false,
            Guid relationshipWithNodeGuid = new Guid(),
            bool relatedNodeIsOnTheLeftSide = true,
            string relationshipName = null,
            bool checkPermission = false,
            bool loadPagesIndividually = false,
            string cacheItemName = null,
            int cacheMinutes = -1,
            string[] cacheDependencies = null)
        {
            var repeaterQuery = (!string.IsNullOrWhiteSpace(classNames) ? new CacheableDocumentQuery(classNames) : new CacheableDocumentQuery());
            if (string.IsNullOrWhiteSpace(path))
            {
                path = "/%";
            }
            repeaterQuery.Path(path);
            repeaterQuery.CacheItemNameParts.Add(path);

            if (nodeId > 0)
            {
                repeaterQuery.WhereEquals("NodeID", nodeId);
                repeaterQuery.CacheItemNameParts.Add(nodeId);
                repeaterQuery.CacheDependencyParts.Add("nodeid|" + nodeId);
            }
            if (documentId > 0)
            {
                repeaterQuery.WhereEquals("DocumentID", documentId);
                repeaterQuery.CacheItemNameParts.Add(documentId);
                repeaterQuery.CacheDependencyParts.Add("documentid|" + documentId);
            }

            repeaterQuery.CombineWithDefaultCulture(combineWithDefaultCulture);

            if (!string.IsNullOrWhiteSpace(cultureCode))
            {
                repeaterQuery.Culture(cultureCode);
                repeaterQuery.CacheItemNameParts.Add(cultureCode);
            }
            if (EnvironmentHelper.PreviewEnabled)
            {
                repeaterQuery.LatestVersion(true);
                repeaterQuery.Published(false);
                repeaterQuery.CacheItemNameParts.Add("LatestVersion");
                repeaterQuery.CacheItemNameParts.Add(false);
            }
            else
            {
                repeaterQuery.PublishedVersion(selectOnlyPublished);
                repeaterQuery.CacheItemNameParts.Add(selectOnlyPublished);
            }
            if (maxRelativeLevel > -1)
            {
                repeaterQuery.NestingLevel(maxRelativeLevel);
                repeaterQuery.CacheItemNameParts.Add(maxRelativeLevel);
            }
            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                repeaterQuery.OrderBy(orderBy);
                repeaterQuery.CacheItemNameParts.Add(orderBy);
            }

            if (selectTopN > -1)
            {
                repeaterQuery.TopN(selectTopN);
                repeaterQuery.CacheItemNameParts.Add(selectTopN);
            }
            if (string.IsNullOrWhiteSpace(siteName))
            {
                siteName = SiteContext.CurrentSiteName;
            }
            if (!string.IsNullOrWhiteSpace(siteName))
            {
                repeaterQuery.OnSite(new SiteInfoIdentifier(siteName));
                repeaterQuery.CacheItemNameParts.Add(siteName);
            }
            if (!string.IsNullOrWhiteSpace(whereCondition))
            {
                repeaterQuery.Where(whereCondition);
                repeaterQuery.CacheItemNameParts.Add(whereCondition);
            }
            if (!string.IsNullOrWhiteSpace(columns))
            {
                repeaterQuery.Columns(columns);
                repeaterQuery.CacheItemNameParts.Add(columns);
            }
            repeaterQuery.FilterDuplicates(filterOutDuplicates);
            if (!string.IsNullOrWhiteSpace(relationshipName))
            {
                repeaterQuery.InRelationWith(relationshipWithNodeGuid, relationshipName, (relatedNodeIsOnTheLeftSide ? RelationshipSideEnum.Left : RelationshipSideEnum.Right));
                repeaterQuery.CacheItemNameParts.Add(relationshipName);
                repeaterQuery.CacheItemNameParts.Add(relationshipWithNodeGuid);
                repeaterQuery.CacheItemNameParts.Add(relatedNodeIsOnTheLeftSide);
            }

            // Handle Caching params
            repeaterQuery.CacheMinutes = cacheMinutes;
            if (!string.IsNullOrWhiteSpace(cacheItemName))
            {
                repeaterQuery.CacheItemNameParts.Add(cacheItemName);
            }
            return repeaterQuery;
        }
    }
}
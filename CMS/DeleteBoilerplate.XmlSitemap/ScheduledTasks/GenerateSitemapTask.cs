using System;
using System.Collections.Generic;
using System.Linq;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.EventLog;
using CMS.Scheduler;
using CMS.SiteProvider;
using DeleteBoilerplate.Common.Extensions;
using DeleteBoilerplate.Domain.Extensions;
using DeleteBoilerplate.Domain.Helpers;
using DeleteBoilerplate.Domain.Models.CmsClasses.SitemapRule;
using DeleteBoilerplate.XmlSitemap.Constants;
using DeleteBoilerplate.XmlSitemap.Helpers;
using DeleteBoilerplate.XmlSitemap.Models;

namespace DeleteBoilerplate.XmlSitemap.ScheduledTasks
{
    public class GenerateSitemapTask : ITask
    {
        public string Execute(TaskInfo task)
        {
            try
            {
                EventLogProvider
                    .LogInformation(nameof(GenerateSitemapTask),
                        "TASK_START",
                        "Staring generate sitemap task");

                var sitemapRules = this.GetSitemapRules();

                var sitemapRuleGroups = sitemapRules.GroupBy(x => x.FileName).ToList();

                var indexSitemap = new SitemapIndex
                {
                    Nodes = new SitemapIndexNode[sitemapRuleGroups.Count]
                };

                foreach (var sitemapRuleGroup in sitemapRuleGroups)
                {
                    var nodes = new List<SitemapNode>();

                    foreach (var sitemapRule in sitemapRuleGroup)
                    {
                        IList<BaseInfo> data;
                        var batch = 0;
                        do
                        {
                            // Execute database request
                            data = this.LoadCmsObjects(sitemapRule, batch * SitemapConstants.CmsObjectsLoadBatchSize);

                            if (data.Count != 0)
                            {
                                if (sitemapRule.RuleTypeEnum == SitemapRuleType.ContainerPage)
                                {
                                    nodes.AddRange(data.Select(x => new SitemapNode
                                    {
                                        Url = ((TreeNode)x)
                                            .RelativeURL
                                            .GetAbsoluteUrl(),
                                        ModifyDate = x.GetDateTimeValue("DocumentModifiedWhen", DateTime.MinValue),
                                        Priority = x.GetDecimalValue("SitemapPriority", 0.5m)
                                    }));
                                }
                                else
                                {
                                    foreach (var info in data)
                                    {
                                        var urls = CustomClassHelper.GetUrls(info);

                                        nodes.AddRange(urls.Select(x => new SitemapNode
                                        {
                                            Url = x.GetAbsoluteUrl(),
                                            ModifyDate = info.GetDateTimeValue(info.TypeInfo.TimeStampColumn, DateTime.MinValue)
                                        }));
                                    }
                                }
                            }
                            
                            batch++;
                        }
                        while (data.Count != 0);
                    }

                    var sitemap = new Sitemap
                    {
                        Nodes = nodes.ToArray()
                    };

                    sitemap.Nodes = sitemap.Nodes
                        .Where(x => !x.Url.IsNullOrWhiteSpace() && x.ModifyDate != DateTime.MinValue)
                        .OrderBy(x => x.Url)
                        .ToArray();

                    var localPath = $"/sitemap/{sitemapRuleGroup.Key}";

                    this.CreateSitemapFileAndDeploy(localPath, sitemap, out var compressedLocalPath);

                    indexSitemap.Nodes[sitemapRuleGroups.IndexOf(sitemapRuleGroup)] = new SitemapIndexNode
                    {
                        Url = compressedLocalPath.GetAbsoluteUrl(),
                        ModifyDate = DateTime.Now
                    };
                }

                const string indexSitemapLocalPath = "/sitemap/sitemap.xml";

                this.CreateSitemapFileAndDeploy(indexSitemapLocalPath, indexSitemap, out _);

                return "Successfully generated";
            }
            catch (Exception e)
            {
                return $"An error occured\n{e.StackTrace}";
            }
        }

        public void CreateSitemapFileAndDeploy<T>(string fileLocalPath, T obj, out string compressedFileLocalPath)
        {
            // Serialize xml and write it to file
            CustomFileHelper.SerializeXmlToFile(fileLocalPath, obj);

            // Create xml compressed version
            compressedFileLocalPath = CustomFileHelper.CompressFile(fileLocalPath);
            
            // Delete uncompressed version
            CustomFileHelper.DeleteFile(fileLocalPath);
            
            // Deploy compressed version to CD servers
            CustomFileHelper.DeployFileToContentDeliveryServers(compressedFileLocalPath, compressedFileLocalPath);
        }

        public IList<SitemapRuleInfo> GetSitemapRules()
        {
            var sitemapRules = SitemapRuleInfoProvider
                .GetSitemapRules()
                .OnSite(SiteContext.CurrentSiteID)
                .ToList();

            foreach (var sitemapRule in sitemapRules)
            {
                var otherRulesContainers = sitemapRules
                    .Where(x => x.RuleType == (int)SitemapRuleType.ContainerPage && x.SitemapRuleID != sitemapRule.SitemapRuleID)
                    .Select(x => x.ContainerPage)
                    .ToList();

                var excludePaths = otherRulesContainers.Where(x => x.StartsWith(sitemapRule.ContainerPage, StringComparison.OrdinalIgnoreCase));

                sitemapRule.ExcludeAliasPaths = excludePaths.ToList();
            }

            return sitemapRules;
        }

        public IList<BaseInfo> LoadCmsObjects(SitemapRuleInfo sitemapRule, int skip)
        {
            var result = new List<BaseInfo>();

            if (sitemapRule.RuleTypeEnum == SitemapRuleType.ContainerPage)
            {
                var query = DocumentHelper
                    .GetDocuments()
                    .Types(RoutingQueryHelper.GetPageTypesWithSeoUrlClassNames())
                    .Columns(DeleteBoilerplate.Domain.Constants.DynamicRouting.SeoUrlFieldName, "DocumentModifiedWhen")
                    .WhereStartsWith("NodeAliasPath", sitemapRule.ContainerPage)
                    .And()
                    .WhereEquals("IncludeInSitemap", true)
                    .AddVersionsParameters(false);

                foreach (var excludePath in sitemapRule.ExcludeAliasPaths)
                {
                    query = query.And().WhereNotStartsWith("NodeAliasPath", excludePath);
                }

                var pages = query
                    .OrderBy(x => x.DocumentID)
                    .Skip(skip)
                    .Take(SitemapConstants.CmsObjectsLoadBatchSize)
                    .ToList();

                result.AddRange(pages);
            }
            else
            {
                var query = CustomClassHelper
                    .GetQuery(sitemapRule.CustomClass, skip, SitemapConstants.CmsObjectsLoadBatchSize);

                if (query != null)
                {
                    var customObjects = query
                        .ToList();

                    result.AddRange(customObjects);
                }
            }

            return result;
        }
    }
}

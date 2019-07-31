using System;
using System.Collections.Generic;
using System.Linq;
using CMS.DocumentEngine.Types.DeleteBoilerplate;
using CMS.Helpers;
using CMS.SiteProvider;

namespace DeleteBoilerplate.Domain.Repositories
{
    public interface ITaxonomyRepository
    {
        IEnumerable<TaxonomyType> GetAllTaxonomyTypes();

        TaxonomyType GeTaxonomyTypeByNodeGuid(string nodeGuid);

        IDictionary<Guid, TaxonomyItem> GetAllTaxonomyItems();

        TaxonomyItem GetTaxonomyItemByNodeGuid(Guid nodeGuid);

        IEnumerable<TaxonomyItem> GetTaxonomyItemsByNodeGuids(IEnumerable<Guid> nodeGuids);

        TaxonomyItem GetTaxonomyItemByNodeGuid(string nodeGuid);

        IEnumerable<TaxonomyItem> GetTaxonomyItemsByNodeGuids(string nodeGuids);
    }

    public class TaxonomyRepository : ITaxonomyRepository
    {
        private readonly string _baseCacheKey = "deleteboilerplate|taxonomy";

        public IEnumerable<TaxonomyType> GetAllTaxonomyTypes()
        {
            var cacheKey = $"{_baseCacheKey}|types|all";

            var result = new List<TaxonomyType>();
            var siteName = SiteContext.CurrentSiteName;

            using (var cs = new CachedSection<List<TaxonomyType>>(ref result, CacheHelper.CacheMinutes(siteName), true, cacheKey))
            {
                if (cs.LoadData)
                {
                    result = TaxonomyTypeProvider.GetTaxonomyTypes().AllCultures()
                        .Published()
                        .OnSite(siteName)
                        .OrderBy("NodeOrder")
                        .ToList();

                    var taxonomyItems = TaxonomyItemProvider.GetTaxonomyItems().AllCultures()
                        .Published()
                        .OnSite(siteName)
                        .OrderBy("NodeOrder")
                        .ToList();

                    foreach (var taxonomyType in result)
                    {
                        taxonomyType.TaxonomyItems =
                            taxonomyItems.Where(x => x.NodeParentID == taxonomyType.NodeID).ToList();

                        foreach (var taxonomyItem in taxonomyType.TaxonomyItems)
                        {
                            taxonomyItem.TaxonomyType = taxonomyType;
                        }
                    }

                    var cacheDependencies = new List<string>
                    {
                        $"nodes|{siteName}|{TaxonomyType.CLASS_NAME}|all",
                        $"nodes|{siteName}|{TaxonomyItem.CLASS_NAME}|all"
                    };

                    cs.Data = result;
                    cs.CacheDependency = CacheHelper.GetCacheDependency(cacheDependencies);
                }
            }

            return result;
        }

        public TaxonomyType GeTaxonomyTypeByNodeGuid(string nodeGuid)
        {
            return GetAllTaxonomyTypes().FirstOrDefault(x =>
                x.NodeGUID == (Guid.TryParse(nodeGuid, out var guid) ? guid : Guid.Empty)
            );
        }

        public IDictionary<Guid, TaxonomyItem> GetAllTaxonomyItems()
        {
            var cacheKey = $"{_baseCacheKey}|items|all";
            var siteName = SiteContext.CurrentSiteName;
            var result = new Dictionary<Guid, TaxonomyItem>();
            using (var cs = new CachedSection<Dictionary<Guid, TaxonomyItem>>(ref result, CacheHelper.CacheMinutes(siteName), true, cacheKey))
            {
                if (cs.LoadData)
                {
                    result = GetAllTaxonomyTypes().SelectMany(x => x.TaxonomyItems)
                        .ToDictionary(k => k.NodeGUID, v => v);

                    var cacheDependencies = new List<string>
                    {
                        $"nodes|{siteName}|{TaxonomyType.CLASS_NAME}|all",
                        $"nodes|{siteName}|{TaxonomyItem.CLASS_NAME}|all"
                    };

                    cs.Data = result;
                    cs.CacheDependency = CacheHelper.GetCacheDependency(cacheDependencies);
                }
            }

            return result;
        }

        public TaxonomyItem GetTaxonomyItemByNodeGuid(Guid nodeGuid)
        {
            var taxonomyDictionary = GetAllTaxonomyItems();

            return taxonomyDictionary.ContainsKey(nodeGuid)
                ? taxonomyDictionary[nodeGuid]
                : null;
        }

        public IEnumerable<TaxonomyItem> GetTaxonomyItemsByNodeGuids(IEnumerable<Guid> nodeGuids)
        {
            return nodeGuids.Join(GetAllTaxonomyItems(), guid => guid, taxonomyItem => taxonomyItem.Key,
                (guid, taxonomyItem) => taxonomyItem.Value);
        }

        public TaxonomyItem GetTaxonomyItemByNodeGuid(string nodeGuid)
        {
            var taxonomyDictionary = GetAllTaxonomyItems();

            return Guid.TryParse(nodeGuid, out var guid) && taxonomyDictionary.ContainsKey(guid)
                ? taxonomyDictionary[guid]
                : null;
        }

        public IEnumerable<TaxonomyItem> GetTaxonomyItemsByNodeGuids(string nodeGuids)
        {
            var guids = nodeGuids.Split(new[] { ",", " ", "|" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => Guid.TryParse(x, out var guid) ? guid : Guid.Empty)
                .Where(x => x != Guid.Empty);

            return guids.Join(GetAllTaxonomyItems(), guid => guid, taxonomyItem => taxonomyItem.Key,
                (guid, taxonomyItem) => taxonomyItem.Value);
        }
    }
}

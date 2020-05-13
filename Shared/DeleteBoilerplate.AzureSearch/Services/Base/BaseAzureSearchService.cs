using CMS.Search;
using CMS.Search.Azure;
using DeleteBoilerplate.AzureSearch.Models.Base;
using DeleteBoilerplate.Common.Extensions;
using DeleteBoilerplate.Common.Helpers;
using DeleteBoilerplate.Domain.Repositories;
using LightInject;
using Microsoft.Azure.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using SearchParameters = Microsoft.Azure.Search.Models.SearchParameters;

namespace DeleteBoilerplate.AzureSearch.Services.Base
{
    public abstract class BaseAzureSearchService<TSearchable, TSearchArgs> : IAzureSearchService<TSearchable, TSearchArgs>
        where TSearchable : class, IAzureSearchResultItem
        where TSearchArgs : BaseAzureSearchArgs
    {
        protected BaseAzureSearchService(string searchIndexName)
        {
            this.SearchIndexName = searchIndexName;
        }

        protected string SearchIndexName { get; set; }
        
        [Inject]
        protected ITaxonomyRepository TaxonomyRepository { get; set; }

        public virtual AzureSearchResult<TSearchable> Find(TSearchArgs args)
        {
            var result = new AzureSearchResult<TSearchable>();

            var filterQuery = this.GetFilterQuery(args);
            var orderQuery = this.GetOrderQuery(args);

            try
            {
                var skip= this.EnsureValidSkipValue(args.Page, args.PageSize);
                
                var searchParameters = new SearchParameters
                {
                    Filter = filterQuery,
                    Skip = skip,
                    Top = args.PageSize,
                    IncludeTotalResultCount = true,
                    OrderBy = orderQuery
                };

                var searchIndexClient = GetSearchClient();

                var indexSearchResult = searchIndexClient?.Documents?.Search<TSearchable>("*", searchParameters);

                result.Items = indexSearchResult?.Results.Select(x => x.Document).ToList();
                result.TotalCount = (int?)indexSearchResult?.Count ?? 0;
                result.IsSuccess = true;
            }
            catch (Exception exception)
            {
                LogHelper.LogException("SEARCH_ERROR", exception);

                result.IsSuccess = false;
            }

            return result;
        }

        protected virtual string GetFilterQuery(TSearchArgs args)
        {
            var queries = new List<string>();
            
            queries.AddRange(this.GetFilterQueryInternal(args));

            if (args.Taxonomies != null && args.Taxonomies.Any())
            {
                var taxonomyFilterQuery = GetTaxonomyFilterQuery(args.Taxonomies);

                if (!string.IsNullOrWhiteSpace(taxonomyFilterQuery))
                {
                    queries.Add(this.GetTaxonomyFilterQuery(args.Taxonomies));
                }
            }

            return string.Join(" and ", queries);
        }

        protected abstract IList<string> GetFilterQueryInternal(TSearchArgs args);
        
        protected abstract IList<string> GetOrderQuery(TSearchArgs args);

        protected virtual ISearchIndexClient GetSearchClient()
        {
            var indexName = NamingHelper.GetValidIndexName(this.SearchIndexName);
            var indexInfo = SearchIndexInfoProvider.GetSearchIndexInfo(indexName);

            if (indexInfo == null)
            {
                return null;
            }

            var serviceClient = new SearchServiceClient(indexInfo.IndexSearchServiceName, new SearchCredentials(indexInfo.IndexAdminKey));

            return serviceClient.Indexes.GetClient(indexName);
        }

        private string GetTaxonomyFilterQuery(IList<Guid> taxonomies)
        {
            const string taxonomyFieldName = "taxonomy";
            var taxonomyCondition = string.Empty;

            if (taxonomies.Count > 0)
            {
                foreach (var type in this.TaxonomyRepository.GetAllTaxonomyTypes())
                {
                    var targetItems = type.TaxonomyItems.Where(x => taxonomies.Contains(x.NodeGUID)).ToList();
                    if (targetItems.Count > 0)
                    {
                        //taxonomy/any(t: search.in(t, 'Guid.ToString(), Guid.ToString()')) search in works like an OR condition
                        var taxonomyGroupCondition = $"{taxonomyFieldName}/any(t: search.in(t, '{string.Join(", ", targetItems.Select(x => $"{x.NodeGUID}"))}'))";

                        if (taxonomyCondition.IsNullOrEmpty())
                        {
                            taxonomyCondition = taxonomyGroupCondition;
                        }
                        else
                        {
                            taxonomyCondition += $" and {taxonomyGroupCondition}";
                        }
                    }
                }
            }

            return taxonomyCondition;
        }

        private int EnsureValidSkipValue(int page, int pageSize)
        {
            const int minValidSkip = 0;
            const int maxValidSkip = 100000;

            int skipItems = (page - 1) * pageSize;

            if (skipItems < minValidSkip)
                skipItems = minValidSkip;

            if (skipItems > maxValidSkip)
                skipItems = maxValidSkip;

            return skipItems;
        }
    }
}

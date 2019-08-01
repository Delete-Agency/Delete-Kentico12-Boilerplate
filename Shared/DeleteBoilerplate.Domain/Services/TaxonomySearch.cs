using System;
using System.Collections.Generic;
using System.Linq;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.Search;
using DeleteBoilerplate.Domain.Models.PageTypes;
using DeleteBoilerplate.Domain.Repositories;
using LightInject;

namespace DeleteBoilerplate.Domain.Services
{
    public interface ITaxonomySearch<out T> where T : TreeNode, IBasePage, new()
    {
        IEnumerable<T> GetItems();

        IEnumerable<T> GetItems(out int totalResults);

        IEnumerable<T> GetItems(IEnumerable<Guid> taxonomies, int skip = 0, int take = Int32.MaxValue,
            string searchSort = "");

        IEnumerable<T> GetItems(IEnumerable<Guid> taxonomies, out int totalResults, int skip = 0,
            int take = Int32.MaxValue, string searchSort = "");
    }

    public class TaxonomySearch<T> : ITaxonomySearch<T>
        where T : TreeNode, IBasePage, new()
    {
        [Inject]
        public TaxonomyRepository TaxonomyRepository { get; set; }

        public IEnumerable<T> GetItems()
        {
            return GetItems(new Guid[] { }, out var totalResults);
        }

        public IEnumerable<T> GetItems(out int totalResults)
        {
            return GetItems(new Guid[] { }, out totalResults);
        }

        public IEnumerable<T> GetItems(IEnumerable<Guid> taxonomies, int skip = 0, int take = Int32.MaxValue, string searchSort = "")
        {
            return GetItems(taxonomies, out var totalResults, skip, take, searchSort);
        }

        public IEnumerable<T> GetItems(IEnumerable<Guid> taxonomies, out int totalResults, int skip = 0, int take = Int32.MaxValue, string searchSort = "")
        {
            var taxonomyCondition = string.Empty;

            var taxonomiesList = taxonomies.ToList();
            if (taxonomiesList.Count > 0)
            {
                foreach (var type in TaxonomyRepository.GetAllTaxonomyTypes())
                {
                    var targetItems = type.TaxonomyItems.Where(x => taxonomiesList.Contains(x.NodeGUID)).ToList();
                    if (targetItems.Count > 0)
                    {
                        taxonomyCondition = SearchSyntaxHelper.AddSearchCondition(taxonomyCondition,
                            SearchSyntaxHelper.GetRequiredCondition(SearchSyntaxHelper.GetFilterCondition(
                                nameof(IBasePage.Taxonomy),
                                targetItems.Select(x => x.NodeGUID.ToString("D")).Join(" OR "))));
                    }
                }
            }

            var condition = SearchSyntaxHelper.CombineSearchCondition(string.Empty,
                new SearchCondition
                {
                    DocumentCondition = new DocumentSearchCondition
                    { ClassNames = new T().ClassName, Culture = Constants.Cultures.Default.Name },
                    ExtraConditions = taxonomyCondition
                });

            var parameters = new SearchParameters
            {
                SearchFor = condition,
                Path = "/%",
                CombineWithDefaultCulture = false,
                CheckPermissions = false,
                SearchInAttachments = false,
                User = MembershipContext.AuthenticatedUser,
                SearchIndexes = Settings.Taxonomy.SearchIndex,
                StartingPosition = skip,
                NumberOfProcessedResults = ((Int64)skip + (Int64)take) >= (Int64)Int32.MaxValue ? Int32.MaxValue : skip + take,
                DisplayResults = take,
                SearchSort = searchSort
            };

            var searchResults = SearchHelper.Search(parameters);
            totalResults = searchResults.Parameters.NumberOfResults;

            return searchResults.Items.Select(x => x.Data).OfType<T>();
        }
    }
}
﻿using System;
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

        IEnumerable<T> GetItems(ITaxonomyExtractSet set, int skip = 0, int take = Int32.MaxValue,
            string searchSort = "");

        IEnumerable<T> GetItems(ITaxonomyExtractSet set, out int totalResults, int skip = 0,
            int take = Int32.MaxValue, string searchSort = "");

        IEnumerable<T> GetItems(IEnumerable<Guid> taxonomies, int skip = 0, int take = Int32.MaxValue,
            string searchSort = "");

        IEnumerable<T> GetItems(IEnumerable<Guid> taxonomies, out int totalResults, int skip = 0,
            int take = Int32.MaxValue, string searchSort = "", string searchRootAliasPath = "/");
    }

    public class TaxonomySearch<T> : ITaxonomySearch<T> where T : TreeNode, IBasePage, new()
    {
        [Inject]
        public ITaxonomyRepository TaxonomyRepository { get; set; }

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

        public IEnumerable<T> GetItems(IEnumerable<Guid> taxonomies, out int totalResults, int skip = 0, int take = Int32.MaxValue, string searchSort = "", string searchRootAliasPath = "/")
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
                Path = $"{searchRootAliasPath}%",
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

        private IEnumerable<Guid> Extract(ITaxonomyExtractSet set)
        {
            var result = new List<Guid>();
            var taxonomyTypes = TaxonomyRepository.GetAllTaxonomyTypes().ToList();
            foreach (var taxonomyType in taxonomyTypes)
            {
                if (set.TaxonomyDictionary.TryGetValue(taxonomyType.Code, out var targetItems))
                {
                    result.AddRange(taxonomyType.TaxonomyItems.Where(i => targetItems == null || !targetItems.Any() || targetItems.Contains(i.Code)).Select(i => i.NodeGUID));
                }
            }
            return result;
        }

        public IEnumerable<T> GetItems(ITaxonomyExtractSet set, out int totalResults, int skip = 0,
            int take = Int32.MaxValue, string searchSort = "")
        {
            return GetItems(Extract(set), out totalResults, skip, take, searchSort);
        }

        public IEnumerable<T> GetItems(ITaxonomyExtractSet set, int skip = 0, int take = Int32.MaxValue, string searchSort = "")
        {
            return GetItems(Extract(set), skip, take, searchSort);
        }

    }
}
using CMS.DocumentEngine;
using CMS.EventLog;
using CMS.Search;
using System;
using System.Linq;

namespace DeleteBoilerplate.Domain.Services
{
    public class SeoUrlPageIndexSearchService
    {
        public static SeoUrlPageIndexSearchResult SearchBySeoUrl(string seoUrl)
        {
            try
            {
                var fieldSearchCondition = SearchSyntaxHelper.GetFieldCondition(Constants.DynamicRouting.SeoUrlFieldName.ToLower(), seoUrl);
                var searchParameters = MakeSearchParameters(fieldSearchCondition);

                var searchResults = SearchHelper.Search(searchParameters);

                if (searchResults.Items?.Any() != true)
                {
                    return SeoUrlPageIndexSearchResult.Fail();
                }

                var foundPage = searchResults.Items.Select(x => new SeoUrlPageIndexSearchResult()
                {
                    ClassName = x.SearchDocument.Get(nameof(TreeNode.ClassName).ToLower()),
                    DocumentId = int.Parse(x.SearchDocument.Get(nameof(TreeNode.DocumentID).ToLower()))
                }).FirstOrDefault();

                return foundPage;
            }
            catch (Exception exception)
            {
                EventLogProvider.LogException(nameof(SeoUrlPageIndexSearchService), "SearchBySeoUrl", exception);
                return SeoUrlPageIndexSearchResult.Fail();
            }
        }

        public static SearchParameters MakeSearchParameters(string searchCondition)
        {
            return new SearchParameters
            {
                SearchFor = searchCondition,
                Path = "/%",
                ClassNames = null,
                CombineWithDefaultCulture = false,
                CheckPermissions = false,
                SearchInAttachments = false,
                DefaultCulture = null,
                SearchIndexes = Constants.SearchIndexes.SearchIndex,
                StartingPosition = 0,
                DisplayResults = 1000,
                NumberOfProcessedResults = 1000,
                AttachmentWhere = null,
                AttachmentOrderBy = null
            };
        }
    }

    public class SeoUrlPageIndexSearchResult
    {
        public int DocumentId { get; set; }

        public string ClassName { get; set; }

        public bool IsSuccess { get; } = true;

        public SeoUrlPageIndexSearchResult()
        {
        }

        protected SeoUrlPageIndexSearchResult(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public static SeoUrlPageIndexSearchResult Fail() => new SeoUrlPageIndexSearchResult(false);
    }
}
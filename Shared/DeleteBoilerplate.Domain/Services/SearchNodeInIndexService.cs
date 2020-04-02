using CMS.DocumentEngine;
using CMS.EventLog;
using CMS.Search;
using System;
using System.Linq;

namespace DeleteBoilerplate.Domain.Services
{
    public class SearchNodeInIndexResult
    {
        public int DocumentId { get; set; }

        public string ClassName { get; set; }

        public bool IsSuccess { get; } = true;

        public SearchNodeInIndexResult()
        {
        }

        protected SearchNodeInIndexResult(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public static SearchNodeInIndexResult Fail() => new SearchNodeInIndexResult(false);
    }

    public class SearchNodeInIndexService
    {
        public static SearchNodeInIndexResult SearchBySeoUrl(string seoUrl)
        {
            try
            {
                var fieldSearchCondition = SearchSyntaxHelper.GetFieldCondition(Constants.DynamicRouting.SeoUrlFieldName.ToLower(), seoUrl);
                var searchParameters = MakeSearchParameters(fieldSearchCondition);

                var searchResults = SearchHelper.Search(searchParameters);

                if (searchResults.Items?.Any() != true)
                {
                    return SearchNodeInIndexResult.Fail();
                }

                var foundNode = searchResults.Items.Select(x => new SearchNodeInIndexResult()
                {
                    ClassName = x.SearchDocument.Get(nameof(TreeNode.ClassName).ToLower()),
                    DocumentId = int.Parse(x.SearchDocument.Get(nameof(TreeNode.DocumentID).ToLower()))
                }).FirstOrDefault();

                return foundNode;
            }
            catch (Exception exception)
            {
                EventLogProvider.LogException(nameof(SearchNodeInIndexService), "SearchBySeoUrl", exception);
                return SearchNodeInIndexResult.Fail();
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
}
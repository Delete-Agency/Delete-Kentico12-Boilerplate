using System.Collections.Generic;

namespace DeleteBoilerplate.AzureSearch.Models.Base
{
    public class AzureSearchResult<TSearchable> where TSearchable : IAzureSearchResultItem
    {
        public AzureSearchResult()
        {
            Items = new List<TSearchable>();
        }

        public IList<TSearchable> Items { get; set; }

        public int TotalCount { get; set; }
        
        public bool IsSuccess { get; set; }
    }
}

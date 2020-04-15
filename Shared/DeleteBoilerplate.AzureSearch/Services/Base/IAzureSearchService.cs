using DeleteBoilerplate.AzureSearch.Models.Base;

namespace DeleteBoilerplate.AzureSearch.Services.Base
{
    public interface IAzureSearchService<TSearchable, TSearchArgs>
        where TSearchable : class, IAzureSearchResultItem
        where TSearchArgs : BaseAzureSearchArgs
    {
        AzureSearchResult<TSearchable> Find(TSearchArgs args);
    }
}

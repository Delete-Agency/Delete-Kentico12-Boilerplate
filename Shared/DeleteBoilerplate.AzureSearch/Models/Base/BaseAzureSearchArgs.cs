using System;
using System.Collections.Generic;

namespace DeleteBoilerplate.AzureSearch.Models.Base
{
    public class BaseAzureSearchArgs
    {
        public int Page { get; set; } = 1;

        public int PageSize { get; set; }

        public IList<Guid> Taxonomies { get; set; }
    }
}

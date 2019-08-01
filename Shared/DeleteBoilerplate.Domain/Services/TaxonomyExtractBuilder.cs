using System.Collections.Generic;

namespace DeleteBoilerplate.Domain.Services
{
    public interface ITaxonomyExtractSet
    {
        IDictionary<string, string[]> TaxonomyDictionary { get; }
    }


    public class TaxonomyExtractBuilder : ITaxonomyExtractSet
    {
        public IDictionary<string, string[]> TaxonomyDictionary { get; set; }

        public TaxonomyExtractBuilder()
        {
            TaxonomyDictionary = new Dictionary<string, string[]>();
        }

        public TaxonomyExtractBuilder AddTaxonomyType(string code)
        {
            TaxonomyDictionary.Add(code,new string[0]);
            return this;
        }

        public TaxonomyExtractBuilder AddTaxonomyItems(string type, params string[] items)
        {
            TaxonomyDictionary.Add(type, items);
            return this;
        }

    }


}

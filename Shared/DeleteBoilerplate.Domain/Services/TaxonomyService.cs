using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using DeleteBoilerplate.Domain.Repositories;

namespace DeleteBoilerplate.Domain.Services
{
    public interface ITaxonomyService
    {
        List<TaxonomyTreeItem> GetTaxonomyTree(string targetTaxonomyTypes = "");

        List<TaxonomyTreeItem> GetTaxonomyTree(string[] targetTaxonomyTypes);
    }

    public class TaxonomyService : ITaxonomyService
    {
        private readonly ITaxonomyRepository _taxonomyRepository;

        public TaxonomyService(ITaxonomyRepository taxonomyRepository)
        {
            _taxonomyRepository = taxonomyRepository;
        }

        public List<TaxonomyTreeItem> GetTaxonomyTree(string targetTaxonomyTypes = "")
        {
            var typesArray = targetTaxonomyTypes
                .Split(new[] {";", " ", "|", ","}, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .ToArray();

            return GetTaxonomyTree(typesArray);
        }

        public List<TaxonomyTreeItem> GetTaxonomyTree(string[] targetTaxonomyTypes)
        {
            var types = _taxonomyRepository.GetAllTaxonomyTypes();
            if (targetTaxonomyTypes != null && targetTaxonomyTypes.Any())
            {
                types = types.Where(x => targetTaxonomyTypes.Contains(x.Title, StringComparer.OrdinalIgnoreCase));
            }

            var result = new List<TaxonomyTreeItem>();
            foreach (var type in types)
            {
                result.Add(
                    new TaxonomyTreeItem
                    {
                        Id = type.NodeGUID.ToString("D"),
                        Name = type.Title
                    });
                result.AddRange(type.TaxonomyItems.Select(x =>
                    new TaxonomyTreeItem
                    {
                        Id = x.NodeGUID.ToString("D"),
                        Parent = type.NodeGUID.ToString("D"),
                        Name = x.Title
                    }));
            }

            return result;
        }
    }


    [JsonObject]
    public class TaxonomyTreeItem
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonIgnore]
        public string Parent { get; set; }

        [JsonProperty("parent")]
        public string SurrogateParent => Parent ?? "#";

        [JsonProperty("text")]
        public string Name { get; set; }
    }
}
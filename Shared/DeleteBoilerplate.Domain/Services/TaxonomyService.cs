using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using DeleteBoilerplate.Domain.Repositories;

namespace DeleteBoilerplate.Domain.Services
{
    public class TaxonomyService 
    {
        private readonly ITaxonomyRepository _taxonomyRepository;

        public TaxonomyService(ITaxonomyRepository taxonomyRepository)
        {
            _taxonomyRepository = taxonomyRepository;
        }

        public string GetTaxonomyTree(string targetTaxonomyType = "")
        {
            var targetTaxonomyTypes = new List<string>();
            var types = _taxonomyRepository.GetAllTaxonomyTypes();
            if (!string.IsNullOrEmpty(targetTaxonomyType))
            {
                targetTaxonomyTypes.AddRange(targetTaxonomyType.Split(';'));
                types = types.Where(x => targetTaxonomyTypes.Contains(x.Title));
            }
            var result = new List<TaxonomyTreeItem>();
            foreach (var type in types)
            {
                result.Add(new TaxonomyTreeItem() { Id = type.NodeGUID.ToString(), Name = type.Title });
                result.AddRange(type.TaxonomyItems.Select(x => new TaxonomyTreeItem()
                { Id = x.NodeGUID.ToString(), Parent = type.NodeGUID.ToString(), Name = x.Title }));
            }
            return JsonConvert.SerializeObject(result);
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
        public string SurrogateParent
        {
            get
            {
                if (Parent == null) return "#";
                return Parent;
            }
        }

        [JsonProperty("text")]
        public string Name { get; set; }
    }
}
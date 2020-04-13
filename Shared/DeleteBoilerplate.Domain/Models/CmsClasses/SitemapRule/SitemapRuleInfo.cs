using System.Collections.Generic;
using DeleteBoilerplate.Domain.Models.CmsClasses.SitemapRule;

namespace DeleteBoilerplate
{
    public partial class SitemapRuleInfo
    {
        public SitemapRuleType RuleTypeEnum => (SitemapRuleType)this.RuleType;

        public IList<string> ExcludeAliasPaths { get; set; } = new List<string>();
    }
}

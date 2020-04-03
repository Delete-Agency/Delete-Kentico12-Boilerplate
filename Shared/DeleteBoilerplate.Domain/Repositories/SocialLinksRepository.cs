using CMS.DocumentEngine.Types.DeleteBoilerplate;
using DeleteBoilerplate.Domain.Extensions;
using System.Collections.Generic;
using System.Linq;
using DeleteBoilerplate.Domain.RepositoryCaching.Attributes;

namespace DeleteBoilerplate.Domain.Repositories
{
    public interface ISocialLinksRepository : IRepository<SocialIcon>
    {
        List<SocialIcon> GetAllSocialIcons();
    }

    public class SocialLinksRepository : ISocialLinksRepository
    {
        [RepositoryCaching]
        public List<SocialIcon> GetAllSocialIcons()
        {
            return SocialIconProvider
                .GetSocialIcons()
                .AddVersionsParameters()
                .OrderBy("NodeOrder")
                .ToList();
        }
    }
}

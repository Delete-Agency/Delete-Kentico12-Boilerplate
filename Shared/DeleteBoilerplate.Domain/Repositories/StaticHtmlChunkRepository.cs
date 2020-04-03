using CMS.DocumentEngine.Types.DeleteBoilerplate;
using DeleteBoilerplate.Domain.RepositoryCaching.Attributes;
using System;
using System.Linq;
using DeleteBoilerplate.Domain.Extensions;

namespace DeleteBoilerplate.Domain.Repositories
{
    public interface IStaticHtmlChunkRepository : IRepository<StaticHtmlChunk>
    {
        StaticHtmlChunk GetByNodeGuid(Guid nodeGuid);
    }

    public class StaticHtmlChunkRepository : IStaticHtmlChunkRepository
    {
        [RepositoryCaching]
        public StaticHtmlChunk GetById(int id)
        {
            return StaticHtmlChunkProvider
                .GetStaticHtmlChunks()
                .AddVersionsParameters()
                .WithID(id)
                .TopN(1)
                .FirstOrDefault();
        }

        [RepositoryCaching]
        public StaticHtmlChunk GetByNodeGuid(Guid nodeGuid)
        {
            return StaticHtmlChunkProvider
                .GetStaticHtmlChunks()
                .AddVersionsParameters()
                .WhereEquals("NodeGUID", nodeGuid)
                .TopN(1)
                .FirstOrDefault();
        }
    }
}
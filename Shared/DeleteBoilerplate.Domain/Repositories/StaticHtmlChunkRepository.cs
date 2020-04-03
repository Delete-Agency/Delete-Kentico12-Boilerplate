using CMS.DocumentEngine.Types.DeleteBoilerplate;
using DeleteBoilerplate.Domain.RepositoryCaching.Attributes;
using System;
using System.Linq;

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
                .WithID(id)
                .TopN(1)
                .FirstOrDefault();
        }

        [RepositoryCaching]
        public StaticHtmlChunk GetByNodeGuid(Guid nodeGuid)
        {
            return StaticHtmlChunkProvider
                .GetStaticHtmlChunks()
                .WhereEquals("NodeGUID", nodeGuid)
                .TopN(1)
                .FirstOrDefault();
        }
    }
}
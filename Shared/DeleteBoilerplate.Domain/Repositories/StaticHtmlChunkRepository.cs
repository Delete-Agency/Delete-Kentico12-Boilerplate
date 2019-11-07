using System;
using System.Linq;
using CMS.DocumentEngine.Types.DeleteBoilerplate;

namespace DeleteBoilerplate.Domain.Repositories
{
    public interface IStaticHtmlChunkRepository
    {
        StaticHtmlChunk GetByNodeGuid(Guid nodeGuid);
    }

    public class StaticHtmlChunkRepository : IStaticHtmlChunkRepository
    {
        public StaticHtmlChunk GetById(int id)
        {
            return StaticHtmlChunkProvider
                .GetStaticHtmlChunks()
                .WithID(id)
                .TopN(1)
                .FirstOrDefault();
        }

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
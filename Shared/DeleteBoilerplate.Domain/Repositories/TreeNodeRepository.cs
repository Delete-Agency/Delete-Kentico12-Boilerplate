using System;
using System.Linq;
using CMS.DocumentEngine;
using DeleteBoilerplate.Domain.Extensions;

namespace DeleteBoilerplate.Domain.Repositories
{
    public interface ITreeNodeRepository : IRepository<TreeNode>
    {
        TreeNode GetByNodeGuid(Guid nodeGuid);
    }

    public class TreeNodeRepository : ITreeNodeRepository
    {
        public TreeNode GetById(int id)
        {
            return DocumentHelper
                .GetDocuments()
                .WithID(id)
                .AddVersionsParameters()
                .TopN(1)
                .FirstOrDefault();
        }

        public TreeNode GetByNodeGuid(Guid nodeGuid)
        {
            // TODO try to remove .WithCoupledColumns(true) 
            return DocumentHelper
                .GetDocuments()
                .WhereEquals("NodeGUID", nodeGuid)
                .AddVersionsParameters()
                .WithCoupledColumns(true)
                .TopN(1)
                .FirstOrDefault();
        }
    }
}

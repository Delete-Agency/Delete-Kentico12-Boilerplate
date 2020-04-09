using CMS.DocumentEngine.Types.DeleteBoilerplate;
using DeleteBoilerplate.Domain.Extensions;
using DeleteBoilerplate.Domain.RepositoryCaching.Attributes;
using System.Collections.Generic;
using System.Linq;

namespace DeleteBoilerplate.Domain.Repositories
{
    public interface IProjectRepository : IRepository<Project>
    {
        List<Project> GetAllProjects();
    }

    public class ProjectRepository : IProjectRepository
    {
        public Project GetById(int id)
        {
            return ProjectProvider
                .GetProjects()
                .WithID(id)
                .AddVersionsParameters()
                .TopN(1)
                .FirstOrDefault();
        }

        [RepositoryCaching]
        public List<Project> GetAllProjects()
        {
            return ProjectProvider
                .GetProjects()
                .AddVersionsParameters()
                .ToList();
        }
    }
}

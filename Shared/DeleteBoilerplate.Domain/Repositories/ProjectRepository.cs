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

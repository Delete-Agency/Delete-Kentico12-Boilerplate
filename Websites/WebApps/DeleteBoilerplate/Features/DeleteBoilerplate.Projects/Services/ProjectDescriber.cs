using CMS.DocumentEngine.Types.DeleteBoilerplate;

namespace DeleteBoilerplate.Projects.Services
{
    public interface IProjectDescriber
    {
        string GetDescribe(Project project);
    }

    public class ProjectDescriber : IProjectDescriber
    {
        public string GetDescribe(Project project)
        {
            return $"{project.Title} : {project.Description}";
        }
    }
}
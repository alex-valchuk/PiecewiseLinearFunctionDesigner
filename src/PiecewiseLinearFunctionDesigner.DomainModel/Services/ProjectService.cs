using PiecewiseLinearFunctionDesigner.DomainModel.Models;

namespace PiecewiseLinearFunctionDesigner.DomainModel.Services
{
    public interface IProjectService
    {
        Project LoadProject(string path);
        
        void SaveProject(Project project);
    }

    public class ProjectService : IProjectService
    {
        public Project LoadProject(string path)
        {
            throw new System.NotImplementedException();
        }

        public void SaveProject(Project project)
        {
            throw new System.NotImplementedException();
        }
    }
}
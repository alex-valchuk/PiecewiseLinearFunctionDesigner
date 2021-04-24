using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PiecewiseLinearFunctionDesigner.Core.Exceptions;
using PiecewiseLinearFunctionDesigner.DomainModel.Exceptions;
using PiecewiseLinearFunctionDesigner.DomainModel.Models;

namespace PiecewiseLinearFunctionDesigner.DomainModel.Services
{
    public interface IProjectService
    {
        void SetActiveProjectFilePath(string filePath);
        
        void SetActiveProject(Project project);

        bool HasActiveProject();
        
        Task<Project> LoadProjectAsync();
        
        Task SaveProjectAsync(Project project, string filePath);
    }

    public class ProjectService : IProjectService
    {
        private const string ProjectExtension = ".plf";
        
        private string _activeProjectFilePath;
        private Project _activeProject;

        public void SetActiveProjectFilePath(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new EmptyStringArgumentException(nameof(filePath));

            if (!ProjectExtension.Equals(Path.GetExtension(filePath), StringComparison.OrdinalIgnoreCase))
                throw new InvalidFileTypeException($"Only files with '{ProjectExtension}' are supported", ProjectExtension);
            
            if (!File.Exists(_activeProjectFilePath))
                throw new InvalidOperationException("The active project file path is invalid.");
            
            _activeProjectFilePath = filePath;
        }

        public void SetActiveProject(Project project)
        {
            _activeProject = project ?? throw new ArgumentNullException(nameof(project));
        }

        public bool HasActiveProject()
        {
            return _activeProject != null;
        }

        public async Task<Project> LoadProjectAsync()
        {
            if (HasActiveProject())
                return _activeProject;
            
            if (string.IsNullOrWhiteSpace(_activeProjectFilePath))
                throw new InvalidOperationException($"You must first specify the active project file path by calling {nameof(SetActiveProjectFilePath)} method.");
            
            if (!File.Exists(_activeProjectFilePath))
                throw new InvalidOperationException("The active project file path is invalid.");

            var projectContent = await File.ReadAllTextAsync(_activeProjectFilePath);
            _activeProject = JsonConvert.DeserializeObject<Project>(projectContent);

            return _activeProject;
        }

        public Task SaveProjectAsync(Project project, string filePath)
        {
            if (project == null)
                throw new ArgumentNullException(nameof(project));
            
            if (string.IsNullOrWhiteSpace(filePath))
                throw new EmptyStringArgumentException(nameof(filePath));

            if (!ProjectExtension.Equals(Path.GetExtension(filePath), StringComparison.OrdinalIgnoreCase))
                throw new InvalidFileTypeException($"Only files with '{ProjectExtension}' are supported", ProjectExtension);

            var projectContent = JsonConvert.SerializeObject(project);
            return File.WriteAllTextAsync(filePath, projectContent);
        }
    }
}
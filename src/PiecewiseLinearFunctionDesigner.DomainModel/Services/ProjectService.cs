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
        Task SetActiveProjectAsync(string filePath);
        
        void SetActiveProject(Project project);

        bool HasActiveProject();
        
        Task<Project> LoadProjectAsync();
        
        Task SaveActiveProjectAsync(string filePath);
    }

    public class ProjectService : IProjectService
    {
        private string _activeProjectFilePath;
        private Project _activeProject;

        public async Task SetActiveProjectAsync(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new EmptyStringArgumentException(nameof(filePath));

            /*if (!ProjectExtension.Equals(Path.GetExtension(filePath), StringComparison.OrdinalIgnoreCase))
                throw new InvalidFileTypeException($"Only files with '{ProjectExtension}' are supported", ProjectExtension);*/
            
            if (!File.Exists(filePath))
                throw new InvalidOperationException("The path is invalid.");

            _activeProjectFilePath = filePath;
            _activeProject = null;

            _activeProject = await LoadProjectAsync();
        }

        public void SetActiveProject(Project project)
        {
            _activeProject = project;
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
                throw new InvalidOperationException($"You must first specify the active project file path by calling {nameof(SetActiveProjectAsync)} method.");
            
            if (!File.Exists(_activeProjectFilePath))
                throw new InvalidOperationException("The active project file path is invalid.");

            try
            {
                var projectContent = await File.ReadAllTextAsync(_activeProjectFilePath);
                _activeProject = JsonConvert.DeserializeObject<Project>(projectContent);

                return _activeProject;
            }
            catch (JsonReaderException ex)
            {
                throw new InvalidFileTypeException(ex.Message);
            }
        }

        public Task SaveActiveProjectAsync(string filePath)
        {
            if (_activeProject == null)
                throw new InvalidOperationException("The active project is not specified.");
            
            if (string.IsNullOrWhiteSpace(filePath))
                throw new EmptyStringArgumentException(nameof(filePath));

            /*if (!ProjectExtension.Equals(Path.GetExtension(filePath), StringComparison.OrdinalIgnoreCase))
                throw new InvalidFileTypeException($"Only files with '{ProjectExtension}' are supported", ProjectExtension);*/

            var projectContent = JsonConvert.SerializeObject(_activeProject);
            return File.WriteAllTextAsync(filePath, projectContent);
        }
    }
}
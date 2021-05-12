using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PiecewiseLinearFunctionDesigner.Core.Exceptions;
using PiecewiseLinearFunctionDesigner.DomainModel.Const;
using PiecewiseLinearFunctionDesigner.DomainModel.Exceptions;
using PiecewiseLinearFunctionDesigner.DomainModel.Models;

namespace PiecewiseLinearFunctionDesigner.DomainModel.Services
{
    public interface IProjectService
    {
        Project ActiveProject { get; }
           
        void AddNewProject();
     
        Task SetActiveProjectAsync(string filePath);
        
        Task SaveActiveProjectAsync(string filePath);
    }

    public class ProjectService : IProjectService
    {
        public Project ActiveProject
        {
            get;
            private set;
        }

        public void AddNewProject()
        {
            ActiveProject = new Project();
        }

        public async Task SetActiveProjectAsync(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new EmptyStringArgumentException(nameof(filePath));

            if (!Defaults.ProjectFileExtension.Equals(Path.GetExtension(filePath), StringComparison.OrdinalIgnoreCase))
                throw new InvalidFileTypeException($"Only '{Defaults.ProjectFileExtension}' files are supported");
            
            if (!File.Exists(filePath))
                throw new InvalidOperationException("The path is invalid.");

            ActiveProject = await LoadProjectAsync(filePath);
        }

        private async Task<Project> LoadProjectAsync(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new InvalidOperationException($"You must first specify the active project file path by calling {nameof(SetActiveProjectAsync)} method.");
            
            if (!File.Exists(filePath))
                throw new InvalidOperationException("The active project file path is invalid.");

            try
            {
                var projectContent = await File.ReadAllTextAsync(filePath);
                ActiveProject = JsonConvert.DeserializeObject<Project>(projectContent);

                return ActiveProject;
            }
            catch (JsonReaderException ex)
            {
                throw new InvalidFileTypeException(ex.Message);
            }
        }

        public Task SaveActiveProjectAsync(string filePath)
        {
            if (ActiveProject == null)
                throw new InvalidOperationException("The active project is not specified.");
            
            if (string.IsNullOrWhiteSpace(filePath))
                throw new EmptyStringArgumentException(nameof(filePath));

            if (!Defaults.ProjectFileExtension.Equals(Path.GetExtension(filePath), StringComparison.OrdinalIgnoreCase))
                throw new InvalidFileTypeException($"Only '{Defaults.ProjectFileExtension}' files are supported");

            var projectContent = JsonConvert.SerializeObject(ActiveProject);
            return File.WriteAllTextAsync(filePath, projectContent);
        }
    }
}
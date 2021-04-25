using System;
using System.Collections.Generic;
using System.Windows;
using InteractiveDataDisplay.Core;
using PiecewiseLinearFunctionDesigner.Core.Events;
using PiecewiseLinearFunctionDesigner.DomainModel.Models;
using PiecewiseLinearFunctionDesigner.DomainModel.Services;
using Prism.Events;
using Prism.Mvvm;
using PiecewiseLinearFunctionDesigner.Localization;

namespace PiecewiseLinearFunctionDesigner.Module.Demonstration.ViewModels
{
    public class FunctionGraphViewModel : BindableBase
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IProjectService _projectService;

        public IList<LineGraph> _lines = new List<LineGraph>();
        public IList<LineGraph> Lines
        {
            get => _lines;
            set => SetProperty(ref _lines, value);
        }

        private Visibility _controlVisibility = Visibility.Collapsed;
        public Visibility ControlVisibility
        {
            get { return _controlVisibility; }
            set => SetProperty(ref _controlVisibility, value);
        }

        private Project _activeProject;
        public Project ActiveProject
        {
            get => _activeProject;
            set => SetProperty(ref _activeProject, value);
        }

        public ITextLocalization TextLocalization { get; }

        public FunctionGraphViewModel(
            IEventAggregator eventAggregator,
            ITextLocalization textLocalization,
            IProjectService projectService)
        {
            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
            _projectService = projectService ?? throw new ArgumentNullException(nameof(projectService));
            TextLocalization = textLocalization ?? throw new ArgumentNullException(nameof(textLocalization));
            
            _eventAggregator.GetEvent<ProjectSpecifiedEvent>().Subscribe(ProjectSpecifiedEventReceived);
            _eventAggregator.GetEvent<FunctionSpecifiedEvent>().Subscribe(FunctionSpecifiedEventReceived);
            _eventAggregator.GetEvent<ProjectClosedEvent>().Subscribe(ProjectClosedEventReceived);
        }

        private async void ProjectSpecifiedEventReceived()
        {
            ActiveProject = await _projectService.LoadProjectAsync();
            ControlVisibility = Visibility.Visible;
        }

        private Function _activeFunction;
        public Function ActiveFunction
        {
            get => _activeFunction;
            set => SetProperty(ref _activeFunction, value);
        }

        private string _functionName;
        private async void FunctionSpecifiedEventReceived(string functionName)
        {
            ActiveProject ??= await _projectService.LoadProjectAsync();
            
            _functionName = functionName;
            ActiveFunction = ActiveProject.GetFunctionByName(functionName);
        }

        private void ProjectClosedEventReceived()
        {
            Lines = null;
            ControlVisibility = Visibility.Collapsed;
        }
    }
}

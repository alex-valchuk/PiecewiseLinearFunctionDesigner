using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
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

        private Function _activeFunction;
        public Function ActiveFunction
        {
            get => _activeFunction;
            set => SetProperty(ref _activeFunction, value);
        }

        public PointCollection PointCollection =>
            new PointCollection(ActiveFunction.Points.Select(p => new System.Windows.Point(p.X, p.Y)));

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

        private async void FunctionSpecifiedEventReceived(string functionName)
        {
            ActiveProject ??= await _projectService.LoadProjectAsync();
            
            ActiveFunction = ActiveProject.GetFunctionByName(functionName);
            ActiveFunction.PropertyChanged += ActiveFunctionOnPropertyChanged;
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(PointCollection)));
        }

        private void ActiveFunctionOnPropertyChanged()
        {
            _eventAggregator.GetEvent<AnyChangeMadeEvent>().Publish();
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(PointCollection)));
        }

        private void ProjectClosedEventReceived()
        {
            ControlVisibility = Visibility.Collapsed;
        }
    }
}

using System;
using System.Collections.Generic;
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
using Point = System.Windows.Point;

namespace PiecewiseLinearFunctionDesigner.Module.Demonstration.ViewModels
{
    public class FunctionGraphViewModel : BindableBase
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IProjectService _projectService;

        private Visibility _controlVisibility = Visibility.Collapsed;
        public Visibility ControlVisibility
        {
            get => _controlVisibility;
            set => SetProperty(ref _controlVisibility, value);
        }

        private Function _activeFunction;
        public Function ActiveFunction
        {
            get => _activeFunction;
            set => SetProperty(ref _activeFunction, value);
        }

        public PointCollection PointCollection =>
            new PointCollection(ActiveFunction?.Points?.Select(p => new Point(p.X, p.Y)) ?? new List<Point>());

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
        }

        private void ProjectSpecifiedEventReceived()
        {
            ActiveFunction = _projectService.ActiveProject.Functions.FirstOrDefault();
            NotifyPointCollectionChanged();
            ControlVisibility = Visibility.Visible;
        }

        private void NotifyPointCollectionChanged()
        {
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(PointCollection)));
        }

        private void FunctionSpecifiedEventReceived(string functionName)
        {
            ActiveFunction = _projectService.ActiveProject.GetFunctionByName(functionName);
            ActiveFunction.PropertyChanged += ActiveFunctionOnPropertyChanged;
            NotifyPointCollectionChanged();
        }

        private void ActiveFunctionOnPropertyChanged()
        {
            _eventAggregator.GetEvent<AnyChangeMadeEvent>().Publish();
            NotifyPointCollectionChanged();
        }
    }
}

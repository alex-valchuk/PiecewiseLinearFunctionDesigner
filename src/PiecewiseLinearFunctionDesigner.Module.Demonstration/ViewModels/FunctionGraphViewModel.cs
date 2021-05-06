using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using InteractiveDataDisplay.Core;
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

        private Visibility _reversedGraphVisibility = Visibility.Collapsed;
        public Visibility ReversedGraphVisibility
        {
            get => _reversedGraphVisibility;
            set => SetProperty(ref _reversedGraphVisibility, value);
        }

        private Function _activeFunction;
        public Function ActiveFunction
        {
            get => _activeFunction;
            set => SetProperty(ref _activeFunction, value);
        }

        public PointCollection PointCollection =>
            new PointCollection(ActiveFunction?.Points?.Select(p => new Point(p.X, p.Y)) ?? new List<Point>());

        public PointCollection ReversedPointCollection =>
            new PointCollection(ActiveFunction?.Points?.Select(p => new Point(p.Y, p.X)) ?? new List<Point>());

        public DataCollection MarkerSources =>
            new DataCollection
            {
                new DataSeries {Key = "X", Data = ActiveFunction?.Xs ?? new double[0]},
                new DataSeries {Key = "Y", Data = ActiveFunction?.Ys ?? new double[0]},
                new ColorSeries(),
                new SizeSeries()
            };

        public DataCollection ReversedMarkerSources =>
            new DataCollection
            {
                new DataSeries {Key = "X", Data = ActiveFunction?.Ys ?? new double[0]},
                new DataSeries {Key = "Y", Data = ActiveFunction?.Xs ?? new double[0]},
                new ColorSeries(),
                new SizeSeries()
            };

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
            _eventAggregator.GetEvent<ReversedFunctionVisibilityChangedEvent>().Subscribe(ReversedFunctionVisibilityChangedEventReceived);
        }

        private void ProjectSpecifiedEventReceived()
        {
            ActiveFunction = _projectService.ActiveProject.Functions.FirstOrDefault();
            NotifyPointCollectionChanged();
            NotifyReversedPointCollectionChanged();
            ControlVisibility = Visibility.Visible;
        }

        private void NotifyPointCollectionChanged()
        {
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(PointCollection)));
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(MarkerSources)));
        }

        private void NotifyReversedPointCollectionChanged(bool visible = false)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(ReversedPointCollection)));
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(ReversedMarkerSources)));
            
            ReversedGraphVisibility = visible
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private void FunctionSpecifiedEventReceived(string functionName)
        {
            ActiveFunction = _projectService.ActiveProject.GetFunctionByName(functionName);
            if (ActiveFunction != null)
            {
                ActiveFunction.PropertyChanged += ActiveFunctionOnPropertyChanged;
            }
            NotifyPointCollectionChanged();
            NotifyReversedPointCollectionChanged();
        }

        private void ActiveFunctionOnPropertyChanged()
        {
            _eventAggregator.GetEvent<AnyChangeMadeEvent>().Publish();
            NotifyPointCollectionChanged();
            NotifyReversedPointCollectionChanged();
        }

        private void ReversedFunctionVisibilityChangedEventReceived(bool visible)
        {
            NotifyReversedPointCollectionChanged(visible);
        }
    }
}

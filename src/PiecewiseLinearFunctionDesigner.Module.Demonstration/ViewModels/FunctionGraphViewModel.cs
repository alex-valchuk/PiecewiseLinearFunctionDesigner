using System;
using System.Collections.Generic;
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
            set
            {
                SetProperty(ref _controlVisibility, value);
            }
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
            var project = await _projectService.LoadProjectAsync();
            Lines = BuildLinesGraph(project.Functions.FirstOrDefault());
            ControlVisibility = Visibility.Visible;
        }

        private static List<LineGraph> BuildLinesGraph(Function function)
        {
            if (function == null)
                return new List<LineGraph>();
            
            var lineGraph = new LineGraph
            {
                Stroke = new SolidColorBrush(Color.FromRgb(255, 25, 25)),
                Description = function.Name,
                StrokeThickness = 2,
                Width = 450,
                Height = 450,
                Points = new PointCollection(function.Points.Select(p => new System.Windows.Point(p.X, p.Y)))
            };

            return new List<LineGraph> {lineGraph};
        }

        private async void FunctionSpecifiedEventReceived(string functionName)
        {
            var project = await _projectService.LoadProjectAsync();
            Lines = BuildLinesGraph(project.GetFunctionByName(functionName));
        }

        private void ProjectClosedEventReceived()
        {
            Lines = null;
            ControlVisibility = Visibility.Collapsed;
        }
    }
}

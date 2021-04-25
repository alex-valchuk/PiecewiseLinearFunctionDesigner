using System;
using System.Collections.ObjectModel;
using PiecewiseLinearFunctionDesigner.Core.Events;
using PiecewiseLinearFunctionDesigner.DomainModel.Models;
using PiecewiseLinearFunctionDesigner.DomainModel.Services;
using PiecewiseLinearFunctionDesigner.Localization;
using Prism.Events;
using Prism.Mvvm;

namespace PiecewiseLinearFunctionDesigner.Module.Declaration.ViewModels
{
    public class PointListViewModel : BindableBase
    { 
        private readonly IEventAggregator _eventAggregator;
        private readonly IProjectService _projectService;
        
        public ITextLocalization TextLocalization { get; }

        private ObservableCollection<Point> _points = new ObservableCollection<Point>();
        public ObservableCollection<Point> Points
        {
            get => _points;
            set => SetProperty(ref _points, value);
        }

        public PointListViewModel(
            IEventAggregator eventAggregator,
            ITextLocalization textLocalization,
            IProjectService projectService)
        {
            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
            TextLocalization = textLocalization ?? throw new ArgumentNullException(nameof(textLocalization));
            _projectService = projectService ?? throw new ArgumentNullException(nameof(projectService));
            
            _eventAggregator.GetEvent<FunctionSpecifiedEvent>().Subscribe(FunctionSpecifiedEventReceived);
        }

        private async void FunctionSpecifiedEventReceived(string selectedFunction)
        {
            var project = await _projectService.LoadProjectAsync();
            var function = project.GetFunctionByName(selectedFunction);
            Points = function?.Points;
        }
    }
}
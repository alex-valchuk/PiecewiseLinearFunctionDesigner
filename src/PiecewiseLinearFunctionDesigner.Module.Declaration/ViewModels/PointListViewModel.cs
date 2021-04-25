using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using PiecewiseLinearFunctionDesigner.Core.Events;
using PiecewiseLinearFunctionDesigner.DomainModel.Models;
using PiecewiseLinearFunctionDesigner.DomainModel.Services;
using PiecewiseLinearFunctionDesigner.Localization;
using Prism.Commands;
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
            set
            {
                SetProperty(ref _points, value);
                PointChangedCommand.Execute();
            }
        }

        public DelegateCommand RowEditEnding;

        public DelegateCommand PointChangedCommand;

        public PointListViewModel(
            IEventAggregator eventAggregator,
            ITextLocalization textLocalization,
            IProjectService projectService)
        {
            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
            TextLocalization = textLocalization ?? throw new ArgumentNullException(nameof(textLocalization));
            _projectService = projectService ?? throw new ArgumentNullException(nameof(projectService));

            PointChangedCommand = new DelegateCommand(HandlePointChangedCommand);
            RowEditEnding = new DelegateCommand(HandleRowEditEnding);
            
            _eventAggregator.GetEvent<FunctionSpecifiedEvent>().Subscribe(FunctionSpecifiedEventReceived);
        }

        private void HandleRowEditEnding()
        {
            _eventAggregator.GetEvent<PointsChangedEvent>().Publish();
        }

        private void HandlePointChangedCommand()
        {
            _eventAggregator.GetEvent<PointsChangedEvent>().Publish();
        }

        private async void FunctionSpecifiedEventReceived(string selectedFunction)
        {
            var project = await _projectService.LoadProjectAsync();

            var function = project.GetFunctionByName(selectedFunction);
            if (function == null)
                return;

            Points = function.Points;
        }
    }
}
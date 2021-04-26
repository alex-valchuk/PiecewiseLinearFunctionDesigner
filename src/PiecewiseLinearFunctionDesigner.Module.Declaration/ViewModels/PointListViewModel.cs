using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using PiecewiseLinearFunctionDesigner.Core.Events;
using PiecewiseLinearFunctionDesigner.DomainModel.Models;
using PiecewiseLinearFunctionDesigner.DomainModel.Services;
using PiecewiseLinearFunctionDesigner.Localization;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Point = PiecewiseLinearFunctionDesigner.DomainModel.Models.Point;

namespace PiecewiseLinearFunctionDesigner.Module.Declaration.ViewModels
{
    public class PointListViewModel : BindableBase
    { 
        private readonly IEventAggregator _eventAggregator;
        private readonly IProjectService _projectService;
        
        public ITextLocalization TextLocalization { get; }

        private Visibility _controlVisibility = Visibility.Collapsed;
        public Visibility ControlVisibility
        {
            get { return _controlVisibility; }
            set
            {
                SetProperty(ref _controlVisibility, value);
            }
        }

        private ObservableCollection<Point> _points = new ObservableCollection<Point>();
        public ObservableCollection<Point> Points
        {
            get => _points;
            set => SetProperty(ref _points, value);
        }

        private int _selectedPoint;
        public int SelectedPoint
        {
            get => _selectedPoint;
            set
            {
                SetProperty(ref _selectedPoint, value);
                DeletePointCommand.RaiseCanExecuteChanged();
            }
        }

        private Function _activeFunction;
        public Function ActiveFunction
        {
            get => _activeFunction;
            set => SetProperty(ref _activeFunction, value);
        }

        public DelegateCommand AddPointCommand { get; }

        public DelegateCommand DeletePointCommand { get; }

        public PointListViewModel(
            IEventAggregator eventAggregator,
            ITextLocalization textLocalization,
            IProjectService projectService)
        {
            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
            TextLocalization = textLocalization ?? throw new ArgumentNullException(nameof(textLocalization));
            _projectService = projectService ?? throw new ArgumentNullException(nameof(projectService));
            
            _eventAggregator.GetEvent<FunctionSpecifiedEvent>().Subscribe(FunctionSpecifiedEventReceived);

            AddPointCommand = new DelegateCommand(ExecuteAddPointCommand);
            DeletePointCommand = new DelegateCommand(ExecuteDeletePointCommand, CanExecuteDeletePointCommand);
        }

        private void FunctionSpecifiedEventReceived(string activeFunction)
        {
            var project = _projectService.ActiveProject;

            ActiveFunction = project.GetFunctionByName(activeFunction);
            Points = new ObservableCollection<Point>(ActiveFunction.Points);
            SelectedPoint = -1;
            ControlVisibility = Visibility.Visible;
        }

        private void ExecuteAddPointCommand()
        {
            var lastPoint = ActiveFunction.Points.LastOrDefault();
            ActiveFunction.AddPoint(new Point
            {
                X = lastPoint?.X ?? 0,
                Y = lastPoint?.Y ?? 0
            });
            Points = new ObservableCollection<Point>(ActiveFunction.Points);
        }

        private void ExecuteDeletePointCommand()
        {
            ActiveFunction.DeletePoint(ActiveFunction.Points[SelectedPoint]);
            Points = new ObservableCollection<Point>(ActiveFunction.Points);
        }

        private bool CanExecuteDeletePointCommand()
        {
            return SelectedPoint >= 0;
        }
    }
}
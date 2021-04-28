using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using PiecewiseLinearFunctionDesigner.Core;
using PiecewiseLinearFunctionDesigner.Core.Events;
using PiecewiseLinearFunctionDesigner.DomainModel.Exceptions;
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
        private readonly IProjectService _projectService;
        private readonly IClipboardService _clipboardService;
        private readonly IPointsConverter _pointsConverter;
        private readonly IMessageService _messageService;
        
        public ITextLocalization TextLocalization { get; }

        private Visibility _controlVisibility = Visibility.Collapsed;
        public Visibility ControlVisibility
        {
            get => _controlVisibility;
            set => SetProperty(ref _controlVisibility, value);
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
        
        public DelegateCommand CopyToClipboardCommand { get; }
        
        public DelegateCommand GetFromClipboardCommand { get; }

        public PointListViewModel(
            IEventAggregator eventAggregator,
            ITextLocalization textLocalization,
            IProjectService projectService,
            IClipboardService clipboardService,
            IPointsConverter pointsConverter,
            IMessageService messageService)
        {
            if (eventAggregator == null) throw new ArgumentNullException(nameof(eventAggregator));
            TextLocalization = textLocalization ?? throw new ArgumentNullException(nameof(textLocalization));
            _projectService = projectService ?? throw new ArgumentNullException(nameof(projectService));
            _clipboardService = clipboardService ?? throw new ArgumentNullException(nameof(clipboardService));
            _pointsConverter = pointsConverter ?? throw new ArgumentNullException(nameof(pointsConverter));
            _messageService = messageService ?? throw new ArgumentNullException(nameof(messageService));

            eventAggregator.GetEvent<ProjectSpecifiedEvent>().Subscribe(ProjectSpecifiedEventReceived);
            eventAggregator.GetEvent<FunctionSpecifiedEvent>().Subscribe(FunctionSpecifiedEventReceived);

            AddPointCommand = new DelegateCommand(ExecuteAddPointCommand);
            DeletePointCommand = new DelegateCommand(ExecuteDeletePointCommand, CanExecuteDeletePointCommand);
            CopyToClipboardCommand = new DelegateCommand(ExecuteCopyToClipboardCommand);
            GetFromClipboardCommand = new DelegateCommand(ExecuteGetFromClipboardCommand);
        }

        private void ProjectSpecifiedEventReceived()
        {
            ActiveFunction = _projectService.ActiveProject.Functions.FirstOrDefault();
            Points = new ObservableCollection<Point>();
            SelectedPoint = -1;
            ControlVisibility = ActiveFunction == null
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        private void FunctionSpecifiedEventReceived(string activeFunction)
        {
            ActiveFunction = _projectService.ActiveProject.GetFunctionByName(activeFunction);
            Points = new ObservableCollection<Point>(ActiveFunction.Points);
            SelectedPoint = -1;
            ControlVisibility = Visibility.Visible;
        }

        private void ExecuteAddPointCommand()
        {
            var lastPoint = ActiveFunction.Points.LastOrDefault();
            ActiveFunction.AddPoint(
                new Point(
                    lastPoint?.X ?? 0,
                    lastPoint?.Y ?? 0));
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

        private void ExecuteCopyToClipboardCommand()
        {
            var str = _pointsConverter.ConvertToString(ActiveFunction.Points);
            _clipboardService.SetText(str);
        }

        private void ExecuteGetFromClipboardCommand()
        {
            try
            {
                var str = _clipboardService.GetText();
                ActiveFunction.Points = _pointsConverter.ConvertToPoints(str);
                Points = new ObservableCollection<Point>(ActiveFunction.Points);
            }
            catch (InvalidDataFormatException)
            {
                _messageService.ShowMessage(TextLocalization.ErrorMessage_ExpectedCsvFormat);
            }
        }
    }
}
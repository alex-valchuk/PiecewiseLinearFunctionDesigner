using System;
using System.Linq;
using System.Windows;
using PiecewiseLinearFunctionDesigner.Core.Events;
using PiecewiseLinearFunctionDesigner.DomainModel.Models;
using PiecewiseLinearFunctionDesigner.DomainModel.Services;
using PiecewiseLinearFunctionDesigner.Localization;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;

namespace PiecewiseLinearFunctionDesigner.Module.Declaration.ViewModels
{
    public class FunctionViewModel : BindableBase
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IProjectService _projectService;

        public ITextLocalization TextLocalization { get; }

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

        private bool _isReversableFunction;
        public bool IsReversableFunction
        {
            get => _isReversableFunction;
            set => SetProperty(ref _isReversableFunction, value);
        }

        private bool _isReverseChecked;
        public bool IsReverseChecked
        {
            get => _isReverseChecked;
            set
            {
                SetProperty(ref _isReverseChecked, value);
                ReverseCheckChangedCommand.Execute();
            }
        }

        public DelegateCommand ReverseCheckChangedCommand { get; }

        public FunctionViewModel(
            IEventAggregator eventAggregator,
            ITextLocalization textLocalization,
            IProjectService projectService)
        {
            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
            TextLocalization = textLocalization ?? throw new ArgumentNullException(nameof(textLocalization));
            _projectService = projectService ?? throw new ArgumentNullException(nameof(projectService));

            ReverseCheckChangedCommand = new DelegateCommand(ExecuteReverseCheckChangedCommand, CanExecuteReverseCheckChangedCommand);

            _eventAggregator.GetEvent<ProjectSpecifiedEvent>().Subscribe(ProjectSpecifiedEventReceived);
            _eventAggregator.GetEvent<FunctionSpecifiedEvent>().Subscribe(FunctionSpecifiedEventReceived);
        }

        private void ExecuteReverseCheckChangedCommand()
        {
            _eventAggregator.GetEvent<ReversedFunctionVisibilityChangedEvent>().Publish(IsReverseChecked);
        }

        private bool CanExecuteReverseCheckChangedCommand()
        {
            return IsReversableFunction;
        }

        private void ProjectSpecifiedEventReceived()
        {
            ActiveFunction = _projectService.ActiveProject.Functions.FirstOrDefault();

            IsReversableFunction = ActiveFunction?.IsReversableFunction ?? false;
            IsReverseChecked = false;
            
            ControlVisibility = ActiveFunction == null
                    ? Visibility.Collapsed
                    : Visibility.Visible;
        }

        private void FunctionSpecifiedEventReceived(string selectedFunction)
        {
            ActiveFunction = _projectService.ActiveProject.GetFunctionByName(selectedFunction);
            if (ActiveFunction != null)
            {
                IsReversableFunction = ActiveFunction.IsReversableFunction;
                ActiveFunction.PropertyChanged += ActiveFunctionOnPropertyChanged;
            }

            IsReverseChecked = false;
            ControlVisibility = Visibility.Visible;
        }

        private void ActiveFunctionOnPropertyChanged()
        {
            IsReversableFunction = ActiveFunction.IsReversableFunction;
            
            if (!IsReversableFunction)
                IsReverseChecked = false;
        }
    }
}
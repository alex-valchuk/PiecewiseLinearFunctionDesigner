using System;
using System.Linq;
using System.Windows;
using PiecewiseLinearFunctionDesigner.Core.Events;
using PiecewiseLinearFunctionDesigner.DomainModel.Models;
using PiecewiseLinearFunctionDesigner.DomainModel.Services;
using PiecewiseLinearFunctionDesigner.Localization;
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

        public FunctionViewModel(
            IEventAggregator eventAggregator,
            ITextLocalization textLocalization,
            IProjectService projectService)
        {
            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
            TextLocalization = textLocalization ?? throw new ArgumentNullException(nameof(textLocalization));
            _projectService = projectService ?? throw new ArgumentNullException(nameof(projectService));

            _eventAggregator.GetEvent<ProjectSpecifiedEvent>().Subscribe(ProjectSpecifiedEventReceived);
            _eventAggregator.GetEvent<FunctionSpecifiedEvent>().Subscribe(FunctionSpecifiedEventReceived);
        }

        private void ProjectSpecifiedEventReceived()
        {
            ActiveFunction = _projectService.ActiveProject.Functions.FirstOrDefault();
            
            ControlVisibility = ActiveFunction == null
                    ? Visibility.Collapsed
                    : Visibility.Visible;
        }

        private void FunctionSpecifiedEventReceived(string selectedFunction)
        {
            ActiveFunction = _projectService.ActiveProject.GetFunctionByName(selectedFunction);
            ControlVisibility = Visibility.Visible;
        }
    }
}
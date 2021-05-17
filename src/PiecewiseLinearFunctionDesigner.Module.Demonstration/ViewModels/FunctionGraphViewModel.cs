using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using PiecewiseLinearFunctionDesigner.Core.Events;
using PiecewiseLinearFunctionDesigner.DomainModel.Models;
using PiecewiseLinearFunctionDesigner.DomainModel.Services;
using Prism.Events;
using Prism.Mvvm;
using PiecewiseLinearFunctionDesigner.Localization;
using PiecewiseLinearFunctionDesigner.Module.Demonstration.Abstract;

namespace PiecewiseLinearFunctionDesigner.Module.Demonstration.ViewModels
{
    public class FunctionGraphViewModel : BindableBase, IFunctionsContainer
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IProjectService _projectService;

        private Visibility _controlVisibility = Visibility.Collapsed;
        public Visibility ControlVisibility
        {
            get => _controlVisibility;
            set => SetProperty(ref _controlVisibility, value);
        }

        public List<Function> Functions
        {
            get;
            private set;
        }

        public event EventHandler FunctionsDefined;

        private Function _activeFunction;
        public Function ActiveFunction
        {
            get => _activeFunction;
            set => SetProperty(ref _activeFunction, value);
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
        }

        private void ProjectSpecifiedEventReceived()
        {
            ActiveFunction = _projectService.ActiveProject.Functions.FirstOrDefault();
            Functions = _projectService.ActiveProject.Functions;
            FunctionsDefined?.Invoke(this, EventArgs.Empty);
            ControlVisibility = Visibility.Visible;
        }

        private void FunctionSpecifiedEventReceived(string functionName)
        {
            ActiveFunction = _projectService.ActiveProject.GetFunctionByName(functionName);
            if (ActiveFunction != null)
            {
                ActiveFunction.PropertyChanged += ActiveFunctionOnPropertyChanged;
            }
            if (Functions != null)
            {
                FunctionsDefined?.Invoke(this, EventArgs.Empty);
            }
        }

        private void ActiveFunctionOnPropertyChanged()
        {
            _eventAggregator.GetEvent<AnyChangeMadeEvent>().Publish();
            FunctionsDefined?.Invoke(this, new EventArgs());
        }
    }
}

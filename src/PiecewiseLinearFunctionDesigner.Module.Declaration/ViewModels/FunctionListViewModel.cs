using System;
using Prism.Events;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using PiecewiseLinearFunctionDesigner.Core.Events;
using PiecewiseLinearFunctionDesigner.DomainModel.Models;
using PiecewiseLinearFunctionDesigner.DomainModel.Services;
using PiecewiseLinearFunctionDesigner.Localization;
using Prism.Commands;

namespace PiecewiseLinearFunctionDesigner.Module.Declaration.ViewModels
{
    public class FunctionListViewModel : BindableBase
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IProjectService _projectService;

        private Visibility _controlVisibility = Visibility.Collapsed;
        public Visibility ControlVisibility
        {
            get => _controlVisibility;
            set => SetProperty(ref _controlVisibility, value);
        }

        public ITextLocalization TextLocalization { get; }
        
        private ObservableCollection<Function> _functions = new ObservableCollection<Function>();
        public ObservableCollection<Function> Functions
        {
            get => _functions;
            set => SetProperty(ref _functions, value);
        }

        private string _selectedFunction;
        public string SelectedFunction
        {
            get => _selectedFunction;
            set
            {
                SetProperty(ref _selectedFunction, value);
                SelectFunctionCommand.Execute();
            }
        }

        public DelegateCommand AddFunctionCommand { get; }
        
        public DelegateCommand SelectFunctionCommand { get; }

        public FunctionListViewModel(
            IEventAggregator eventAggregator,
            ITextLocalization textLocalization,
            IProjectService projectService)
        {
            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
            _projectService = projectService ?? throw new ArgumentNullException(nameof(projectService));
            TextLocalization = textLocalization ?? throw new ArgumentNullException(nameof(textLocalization));

            _eventAggregator.GetEvent<ProjectSpecifiedEvent>().Subscribe(ProjectSpecifiedEventReceived);

            AddFunctionCommand = new DelegateCommand(HandleAddFunctionCommand);
            SelectFunctionCommand = new DelegateCommand(HandleSelectFunctionCommand);
        }

        private void ProjectSpecifiedEventReceived()
        {
            Functions = new ObservableCollection<Function>(_projectService.ActiveProject.Functions);
            if (Functions.Any())
            {
                SelectedFunction = Functions.First().Name;
                _eventAggregator.GetEvent<FunctionSpecifiedEvent>().Publish(SelectedFunction);
            }
            
            ControlVisibility = Visibility.Visible;
        }

        private void HandleAddFunctionCommand()
        {
            var project = _projectService.ActiveProject;
            project.AddNewFunction(GetNewFunctionName(project));

            Functions = new ObservableCollection<Function>(project.Functions);
            SelectedFunction = project.Functions.Last().Name;
            
            _eventAggregator.GetEvent<FunctionSpecifiedEvent>().Publish(SelectedFunction);
            _eventAggregator.GetEvent<AnyChangeMadeEvent>().Publish();
        }

        private string GetNewFunctionName(Project project, int attempt = 0)
        {
            int functionNumber = project.Functions.Count + 1 + attempt;
            var functionName = $"{TextLocalization.Function} {functionNumber}";
            if (project.GetFunctionByName(functionName) == null)
                return functionName;

            return GetNewFunctionName(project, ++attempt);
        }

        private void HandleSelectFunctionCommand()
        {
            _eventAggregator.GetEvent<FunctionSpecifiedEvent>().Publish(SelectedFunction);
        }
    }
}

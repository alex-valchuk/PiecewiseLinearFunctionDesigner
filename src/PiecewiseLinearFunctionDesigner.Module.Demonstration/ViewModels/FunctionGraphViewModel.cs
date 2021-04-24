using System;
using System.Collections.Generic;
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
        private readonly ITextLocalization _textLocalization;
        private readonly IProjectService _projectService;
        
        public IReadOnlyList<Function> Functions { get; private set; }

        public FunctionGraphViewModel(
            IEventAggregator eventAggregator,
            ITextLocalization textLocalization,
            IProjectService projectService)
        {
            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
            _textLocalization = textLocalization ?? throw new ArgumentNullException(nameof(textLocalization));
            _projectService = projectService ?? throw new ArgumentNullException(nameof(projectService));
            
            _eventAggregator.GetEvent<ProjectSpecifiedEvent>().Subscribe(ProjectSpecifiedEventReceived);
        }

        private async void ProjectSpecifiedEventReceived()
        {
            var project = await _projectService.LoadProjectAsync();
            Functions = project.Functions;
        }
    }
}

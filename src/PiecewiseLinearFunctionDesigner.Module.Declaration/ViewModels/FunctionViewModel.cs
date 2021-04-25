﻿using System;
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

        private Function _selectedFunction;
        public Function SelectedFunction
        {
            get => _selectedFunction;
            private set => SetProperty(ref _selectedFunction, value);
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

        public FunctionViewModel(
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
            SelectedFunction = project.GetFunctionByName(selectedFunction);
            ControlVisibility = Visibility.Visible;
        }
    }
}
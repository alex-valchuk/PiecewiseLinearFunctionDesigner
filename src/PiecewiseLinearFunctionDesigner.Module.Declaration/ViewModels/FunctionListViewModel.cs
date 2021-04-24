using System;
using Prism.Events;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Linq;
using PiecewiseLinearFunctionDesigner.Core;
using PiecewiseLinearFunctionDesigner.Core.Const;
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
        private readonly IMessageService _messageService;

        public ITextLocalization TextLocalization { get; }
        
        private ObservableCollection<Function> _functions = new ObservableCollection<Function>();
        public ObservableCollection<Function> Functions
        {
            get { return _functions; }
            set { SetProperty(ref _functions, value); }
        }

        public DelegateCommand AddFunctionCommand { get; }

        public FunctionListViewModel(
            IEventAggregator eventAggregator,
            ITextLocalization textLocalization,
            IProjectService projectService,
            IMessageService messageService)
        {
            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
            _projectService = projectService ?? throw new ArgumentNullException(nameof(projectService));
            _messageService = messageService ?? throw new ArgumentNullException(nameof(messageService));
            TextLocalization = textLocalization ?? throw new ArgumentNullException(nameof(textLocalization));

            AddFunctionCommand = new DelegateCommand(HandleAddFunctionCommand);

            _eventAggregator.GetEvent<MessageSentEvent>().Subscribe(AddFunctionMessageReceived, ThreadOption.PublisherThread, false, filter => filter.StartsWith(MessageMarkers.NewFunction));
            _eventAggregator.GetEvent<ProjectSpecifiedEvent>().Subscribe(ProjectSpecifiedEventReceived);
        }

        private void HandleAddFunctionCommand()
        {
            throw new NotImplementedException();
        }

        private void AddFunctionMessageReceived(string newFunctionMessage)
        {
            var functionName = newFunctionMessage.Substring(MessageMarkers.NewFunction.Length);
            if (Functions.All(f => !f.Name.Equals(functionName)))
            {
                Functions.Add(new Function
                {
                    Name = functionName,
                    Enabled = true
                });
                _eventAggregator.GetEvent<MessageSentEvent>().Publish(MessageMarkers.AnyChangeMade);
            }
            else
            {
                _messageService.ShowMessage(string.Format(TextLocalization.FunctionWithNameAlreadyAdded, functionName));
            }
        }

        private async void ProjectSpecifiedEventReceived()
        {
            var project = await _projectService.LoadProjectAsync();
            Functions = new ObservableCollection<Function>(project.Functions);
        }
    }
}

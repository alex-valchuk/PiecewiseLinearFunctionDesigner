using System;
using Prism.Events;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using PiecewiseLinearFunctionDesigner.Core;
using PiecewiseLinearFunctionDesigner.Core.Const;
using PiecewiseLinearFunctionDesigner.DomainModel.Models;
using PiecewiseLinearFunctionDesigner.Localization;
using Prism.Commands;

namespace PiecewiseLinearFunctionDesigner.Module.Declaration.ViewModels
{
    public class FunctionListViewModel : BindableBase
    {
        private readonly IEventAggregator _eventAggregator;

        public ITextLocalization TextLocalization { get; }
        
        private ObservableCollection<Function> _functions;
        public ObservableCollection<Function> Functions
        {
            get { return _functions; }
            set { SetProperty(ref _functions, value); }
        }

        public DelegateCommand AddFunctionCommand { get; }

        public FunctionListViewModel(
            IEventAggregator eventAggregator,
            ITextLocalization textLocalization)
        {
            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
            TextLocalization = textLocalization ?? throw new ArgumentNullException(nameof(textLocalization));

            Functions = new ObservableCollection<Function>();
            AddFunctionCommand = new DelegateCommand(HandleAddFunctionCommand);

            _eventAggregator.GetEvent<MessageSentEvent>().Subscribe(AddFunctionMessageReceived, ThreadOption.PublisherThread, false, filter => filter.StartsWith(MessageMarkers.NewFunction));
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
                MessageBox.Show(string.Format(TextLocalization.FunctionWithNameAlreadyAdded, functionName));
            }
        }
    }
}

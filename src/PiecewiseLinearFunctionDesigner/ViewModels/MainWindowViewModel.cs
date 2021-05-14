using System;
using PiecewiseLinearFunctionDesigner.Abstract;
using PiecewiseLinearFunctionDesigner.Core.Events;
using PiecewiseLinearFunctionDesigner.Localization;
using Prism.Events;
using Prism.Mvvm;

namespace PiecewiseLinearFunctionDesigner.ViewModels
{
    public class MainWindowViewModel : BindableBase, IClosing
    {
        private readonly IEventAggregator _eventAggregator;
        public ITextLocalization TextLocalization { get; }

        public MainWindowViewModel(
            IEventAggregator eventAggregator,
            ITextLocalization textLocalization)
        {
            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
            TextLocalization = textLocalization ?? throw new ArgumentNullException(nameof(textLocalization));
        }

        public bool OnClosing()
        {
            _eventAggregator.GetEvent<AppClosingEvent>().Publish();
            return true;
        }
    }
}

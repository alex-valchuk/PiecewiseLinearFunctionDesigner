using System;
using PiecewiseLinearFunctionDesigner.Localization;
using Prism.Mvvm;

namespace PiecewiseLinearFunctionDesigner.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        public ITextLocalization TextLocalization { get; }

        public MainWindowViewModel(ITextLocalization textLocalization)
        {
            TextLocalization = textLocalization ?? throw new ArgumentNullException(nameof(textLocalization));
        }
    }
}

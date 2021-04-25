using System;
using System.Collections.ObjectModel;
using System.Linq;
using Prism.Mvvm;

namespace PiecewiseLinearFunctionDesigner.DomainModel.Models
{
    public class Project : BindableBase
    {
        private ObservableCollection<Function> _functions = new ObservableCollection<Function>();
        public ObservableCollection<Function> Functions
        {
            get => _functions;
            set => SetProperty(ref _functions, value);
        }

        public Function GetFunctionByName(string functionName) =>
            Functions.FirstOrDefault(f => f.Name.Equals(functionName, StringComparison.InvariantCultureIgnoreCase));
    }
}
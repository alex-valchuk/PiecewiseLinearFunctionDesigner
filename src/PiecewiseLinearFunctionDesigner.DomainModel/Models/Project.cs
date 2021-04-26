using System;
using System.Collections.Generic;
using System.Linq;
using Prism.Mvvm;

namespace PiecewiseLinearFunctionDesigner.DomainModel.Models
{
    public class Project : BindableBase
    {
        private List<Function> _functions = new List<Function>();
        public List<Function> Functions
        {
            get => _functions;
            set => SetProperty(ref _functions, value);
        }

        public Function GetFunctionByName(string functionName) =>
            Functions.FirstOrDefault(f => f.Name.Equals(functionName, StringComparison.InvariantCultureIgnoreCase));

        public void AddNewFunction(string functionName)
        {
            Functions.Add(new Function
            {
                Name = functionName
            });
        }
    }
}
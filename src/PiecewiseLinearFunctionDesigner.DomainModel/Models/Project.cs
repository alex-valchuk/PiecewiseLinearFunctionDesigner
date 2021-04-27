using System;
using System.Collections.Generic;
using System.Linq;

namespace PiecewiseLinearFunctionDesigner.DomainModel.Models
{
    public class Project
    {
        private List<Function> _functions = new List<Function>();
        public List<Function> Functions
        {
            get => _functions;
            set => _functions = value;
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
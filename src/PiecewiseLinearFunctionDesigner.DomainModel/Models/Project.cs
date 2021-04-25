using System;
using System.Collections.Generic;
using System.Linq;

namespace PiecewiseLinearFunctionDesigner.DomainModel.Models
{
    public class Project
    {
        public IList<Function> Functions { get; set; } = new List<Function>();

        public Function GetFunctionByName(string functionName) =>
            Functions.FirstOrDefault(f => f.Name.Equals(functionName, StringComparison.InvariantCultureIgnoreCase));
    }
}
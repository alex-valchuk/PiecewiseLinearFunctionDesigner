using System;
using System.Collections.Generic;
using PiecewiseLinearFunctionDesigner.DomainModel.Models;

namespace PiecewiseLinearFunctionDesigner.Module.Demonstration.Abstract
{
    public interface IFunctionsContainer
    {
        public List<Function> Functions { get; }
        
        public Function ActiveFunction { get; }

        event EventHandler FunctionsDefined;
    }
}
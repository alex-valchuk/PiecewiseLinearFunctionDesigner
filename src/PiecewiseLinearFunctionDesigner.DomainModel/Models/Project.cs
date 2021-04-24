using System.Collections.Generic;

namespace PiecewiseLinearFunctionDesigner.DomainModel.Models
{
    public class Project
    {
        public IReadOnlyList<Function> Functions { get; set; } = new List<Function>();
    }
}
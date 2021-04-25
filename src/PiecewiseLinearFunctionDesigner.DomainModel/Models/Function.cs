using System.Collections.Generic;

namespace PiecewiseLinearFunctionDesigner.DomainModel.Models
{
    public class Function
    {
        public string Name { get; set; }

        public IReadOnlyList<Point> Points { get; set; } = new List<Point>();
    }
}
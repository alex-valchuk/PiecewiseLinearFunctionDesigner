using System;
using System.Collections.Generic;
using System.Text;
using PiecewiseLinearFunctionDesigner.DomainModel.Models;

namespace PiecewiseLinearFunctionDesigner.DomainModel.Services
{
    public interface IPointsConverter
    {
        string ConvertToString(List<Point> points);
        List<Point> ConvertToPoints(string str);
    }

    public class CsvPointsConverter : IPointsConverter
    {
        public string ConvertToString(List<Point> points)
        {
            if (points == null)
                throw new ArgumentNullException(nameof(points));
            
            var resultBuilder = new StringBuilder("X;Y");
            resultBuilder.AppendLine();
            
            foreach (var point in points)
            {
                resultBuilder.AppendLine($"`{point.X};`{point.Y}");
            }

            return resultBuilder.ToString();
        }

        public List<Point> ConvertToPoints(string str)
        {
            throw new NotImplementedException();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using PiecewiseLinearFunctionDesigner.DomainModel.Exceptions;
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
        private const string NegativeNumberEscapeSign = "'";
        private const char ColumnDelimiter = ';';
        
        public string ConvertToString(List<Point> points)
        {
            if (points == null)
                throw new ArgumentNullException(nameof(points));
            
            var resultBuilder = new StringBuilder($"X{ColumnDelimiter}Y");
            resultBuilder.AppendLine();
            
            foreach (var point in points)
            {
                var xStr = GetCsvCompliantDouble(point.X);
                var yStr = GetCsvCompliantDouble(point.Y);
            
                resultBuilder.AppendLine($"{xStr}{ColumnDelimiter}{yStr}");
            }

            return resultBuilder.ToString();
        }

        private static string GetCsvCompliantDouble(double value)
            => value >= 0
                ? Convert.ToString(value, CultureInfo.InvariantCulture)
                : $"{NegativeNumberEscapeSign}{value}";

        public List<Point> ConvertToPoints(string str)
        {
            using var stringReader = new StringReader(str);
            var line = stringReader.ReadLine() ?? "";
            
            var lineParts = line.Split(ColumnDelimiter);
            if (lineParts.Length != 2)
                throw new InvalidDataFormatException($"Every line must be split by '{ColumnDelimiter}' and have 2 parts.");
            
            if (!lineParts[0].Equals(nameof(Point.X), StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataFormatException($"First line must start from {nameof(Point.X)}.");
            
            if (!lineParts[1].Equals(nameof(Point.Y), StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataFormatException($"First line must end with from {nameof(Point.Y)}.");

            var points = new List<Point>();
            
            while (true)
            {
                line = stringReader.ReadLine();
                if (string.IsNullOrWhiteSpace(line))
                    break;

                lineParts = line.Split(ColumnDelimiter);
                if (lineParts.Length != 2)
                    throw new InvalidDataFormatException($"Every line must be split by '{ColumnDelimiter}' and have 2 parts.");
                
                if (!double.TryParse(lineParts[0].Replace(NegativeNumberEscapeSign, ""), out var x))
                    throw new InvalidDataFormatException($"{nameof(Point.X)} must be of type double.");
                
                if (!double.TryParse(lineParts[1].Replace(NegativeNumberEscapeSign, ""), out var y))
                    throw new InvalidDataFormatException($"{nameof(Point.X)} must be of type double.");
                
                points.Add(new Point(x, y));
            }
            
            return points;
        }
    }
}
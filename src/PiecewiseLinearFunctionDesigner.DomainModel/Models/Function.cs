using System;
using System.Collections.Generic;
using System.Linq;

namespace PiecewiseLinearFunctionDesigner.DomainModel.Models
{
    public class Function
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                PropertyChanged?.Invoke();
            }
        }

        private List<Point> _points = new List<Point>();
        public List<Point> Points
        {
            get => _points;
            set => _points = value.ToList();
        }

        public Function()
        {
            Point.PropertyChanged += () => PropertyChanged?.Invoke();
        }

        public void AddPoint(Point point)
        {
            if (point == null)
                throw new ArgumentNullException(nameof(point));
            
            _points.Add(point);
            PropertyChanged?.Invoke();
        }

        public void DeletePoint(Point point)
        {
            if (point == null)
                throw new ArgumentNullException(nameof(point));

            _points.Remove(point);
            PropertyChanged?.Invoke();
        }
    }
}
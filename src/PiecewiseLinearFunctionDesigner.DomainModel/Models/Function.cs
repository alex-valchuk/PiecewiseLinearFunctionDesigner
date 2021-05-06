using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace PiecewiseLinearFunctionDesigner.DomainModel.Models
{
    public class Function
    {
        private readonly Func<double, double, bool> _fallingOrEqualToFail = (d1, d2) => d1 <= d2;
        private readonly Func<double, double, bool> _growingOrEqualToFail = (d1, d2) => d1 >= d2;
                
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

        [JsonIgnore]
        public double[] Xs => Points?.Select(p => p.X).ToArray();

        [JsonIgnore]
        public double[] Ys => Points?.Select(p => p.Y).ToArray();

        [JsonIgnore]
        public bool IsReversableFunction
        {
            get
            {
                if (Points.Count == 0 || Points.Count == 1)
                    return false;

                return IsFromLeftToRightGrowingReversableFunction ||
                       IsFromLeftToRightFallingReversableFunction ||
                       IsFromRightToLeftFallingReversableFunction ||
                       IsFromRightToLeftGrowingReversableFunction;
            }
        }

        [JsonIgnore]
        public bool IsFromLeftToRightGrowingReversableFunction
        {
            get
            {
                if (Xs.First() < Xs.Last() && Ys.First() < Ys.Last())
                {
                    return DirectionConfirmed(Xs, _fallingOrEqualToFail) && 
                           DirectionConfirmed(Ys, _fallingOrEqualToFail);
                }

                return false;
            }
        }

        private bool DirectionConfirmed(double[] items, Func<double, double, bool> conditionToFail)
        {
            for (int i = 1; i < items.Length; i++)
            {
                if (conditionToFail(items[i], items[i - 1]))
                    return false;
            }

            return true;
        }

        [JsonIgnore]
        public bool IsFromLeftToRightFallingReversableFunction
        {
            get
            {
                if (Xs.First() < Xs.Last() && Ys.First() > Ys.Last())
                {
                    return DirectionConfirmed(Xs, _fallingOrEqualToFail) && 
                           DirectionConfirmed(Ys, _growingOrEqualToFail);
                }

                return false;
            }
        }

        [JsonIgnore]
        public bool IsFromRightToLeftFallingReversableFunction
        {
            get
            {
                if (Xs.First() > Xs.Last() && Ys.First() > Ys.Last())
                {
                    return DirectionConfirmed(Xs, _growingOrEqualToFail) && 
                           DirectionConfirmed(Ys, _growingOrEqualToFail);
                }

                return false;
            }
        }

        [JsonIgnore]
        public bool IsFromRightToLeftGrowingReversableFunction
        {
            get
            {
                if (Xs.First() > Xs.Last() && Ys.First() < Ys.Last())
                {
                    return DirectionConfirmed(Xs, _growingOrEqualToFail) && 
                           DirectionConfirmed(Ys, _fallingOrEqualToFail);
                }

                return false;
            }
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
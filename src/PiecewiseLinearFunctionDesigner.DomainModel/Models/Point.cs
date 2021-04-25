using Prism.Mvvm;

namespace PiecewiseLinearFunctionDesigner.DomainModel.Models
{
    public class Point : BindableBase
    {
        private double _x;

        public double X
        {
            get => _x;
            set => SetProperty(ref _x, value);
        }

        private double _y;

        public double Y
        {
            get => _y;
            set => SetProperty(ref _y, value);
        }
    }
}
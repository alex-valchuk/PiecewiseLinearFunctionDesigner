namespace PiecewiseLinearFunctionDesigner.DomainModel.Models
{
    public delegate void PropertyChangedEventHandler();
    
    public class Point
    {
        public static event PropertyChangedEventHandler PropertyChanged;

        public Point(double x, double y)
        {
            _x = x;
            _y = y;
        }

        private double _x;
        public double X
        {
            get => _x;
            set
            {
                _x = value;
                PropertyChanged?.Invoke();
            }
        }

        private double _y;
        public double Y
        {
            get => _y;
            set
            {
                _y = value;
                PropertyChanged?.Invoke();
            }
        }
    }
}
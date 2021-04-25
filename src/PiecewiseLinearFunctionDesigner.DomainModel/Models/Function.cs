using System.Collections.ObjectModel;
using Prism.Mvvm;

namespace PiecewiseLinearFunctionDesigner.DomainModel.Models
{
    public class Function : BindableBase
    {
        private string _name;

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private ObservableCollection<Point> _points = new ObservableCollection<Point>();
        public ObservableCollection<Point> Points
        {
            get => _points;
            set => SetProperty(ref _points, value);
        }
    }
}
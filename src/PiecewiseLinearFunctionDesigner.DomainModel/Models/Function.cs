using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json.Serialization;
using System.Windows.Media;
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

        [JsonIgnore]
        public PointCollection PointCollection =>
            new PointCollection(Points.Select(p => new System.Windows.Point(p.X, p.Y)));
    }
}
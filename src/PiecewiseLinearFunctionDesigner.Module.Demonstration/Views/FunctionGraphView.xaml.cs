using System;
using System.Windows.Controls;
using System.Windows.Media;
using InteractiveDataDisplay.Core;
using PiecewiseLinearFunctionDesigner.Localization;
using PiecewiseLinearFunctionDesigner.Module.Demonstration.Abstract;

namespace PiecewiseLinearFunctionDesigner.Module.Demonstration.Views
{
    public partial class FunctionGraphView : UserControl
    {
        private readonly ITextLocalization _textLocalization;
        private readonly IFunctionsContainer _functionsContainer;

        private Color[] _functionColors = new[]
        {
            Colors.Red,
            Colors.Green,
            Colors.Blue,
            Colors.Brown,
            Colors.Crimson,
            Colors.Fuchsia,
            Colors.Gold
        };
        
        public FunctionGraphView(ITextLocalization textLocalization)
        {
            _textLocalization = textLocalization ?? throw new ArgumentNullException(nameof(textLocalization));
            InitializeComponent();
            
            if (DataContext is IFunctionsContainer functionsContainer)
            {
                _functionsContainer = functionsContainer;
                _functionsContainer.FunctionsDefined += FunctionsContainerOnFunctionsDefined;
            }
        }

        private void FunctionsContainerOnFunctionsDefined(object sender, EventArgs e)
        {
            graphs.Children.Clear();
            
            var i = 0;
            foreach (var function in _functionsContainer.Functions)
            {
                i = i < _functionColors.Length
                    ? i
                    : 0;
                AddFunctionGraph(function.Name, function.Xs, function.Ys, _functionColors[i++]);

                if (function == _functionsContainer.ActiveFunction && function.IsReversableFunction)
                {
                    AddFunctionGraph(_textLocalization.Reverse + " " + function.Name, function.Ys, function.Xs, Colors.Orange);
                }
            }
        }

        private void AddFunctionGraph(string functionName, double[] xs, double[] ys, Color functionColor)
        {
            var lg = new LineGraph
            {
                Stroke = new SolidColorBrush(functionColor),
                Description = functionName,
                StrokeThickness = 2
            };
            graphs.Children.Add(lg);

            lg.Plot(xs, ys);

            var mg = new CircleMarkerGraph
            {
                Sources = new DataCollection
                {
                    new DataSeries {Key = "X", Data = xs ?? new double[0]},
                    new DataSeries {Key = "Y", Data = ys ?? new double[0]},
                    new ColorSeries(),
                    new SizeSeries()
                },
                Color = Colors.Black,
                Min = 1,
                Max = 20
            };

            graphs.Children.Add(mg);
        }
    }
}

using System.ComponentModel;
using System.Windows;
using PiecewiseLinearFunctionDesigner.Abstract;

namespace PiecewiseLinearFunctionDesigner.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            if (DataContext is IClosing context)
            {
                e.Cancel = !context.OnClosing();
            }
        }
    }
}

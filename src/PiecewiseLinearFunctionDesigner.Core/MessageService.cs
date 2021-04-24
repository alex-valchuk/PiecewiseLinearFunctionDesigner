using System.Windows;

namespace PiecewiseLinearFunctionDesigner.Core
{
    public interface IMessageService
    {
        void ShowMessage(string message);

        bool ActionConfirmed(string message, string caption);
    }

    public class MessageService : IMessageService
    {
        public void ShowMessage(string message)
        {
            MessageBox.Show(message);
        }

        public bool ActionConfirmed(string message, string caption)
        {
            return MessageBox.Show(
                message,
                caption,
                MessageBoxButton.YesNo) == MessageBoxResult.Yes;
        }
    }
}
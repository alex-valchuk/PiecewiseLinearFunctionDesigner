using System.Windows;

namespace PiecewiseLinearFunctionDesigner.Core
{
    public interface IClipboardService
    {
        string GetText();
        void SetText(string text);
    }

    public class ClipboardService : IClipboardService
    {
        public string GetText() => Clipboard.GetText();

        public void SetText(string text) => Clipboard.SetText(text);
    }
}
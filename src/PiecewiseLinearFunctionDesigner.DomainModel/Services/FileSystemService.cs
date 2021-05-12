using Microsoft.Win32;

namespace PiecewiseLinearFunctionDesigner.DomainModel.Services
{
    public interface IFileSystemService
    {
        bool OpenFile(string extensionDescription, string extension, out string selectedFile);

        bool SaveFile(string extensionDescription, string extension, out string filePath);
    }

    public class FileSystemService : IFileSystemService
    {
        private readonly OpenFileDialog _openFileDialog = new OpenFileDialog();
        private readonly SaveFileDialog _saveFileDialog = new SaveFileDialog();

        public bool OpenFile(string extensionDescription, string extension, out string selectedFile)
        {
            if (OpenFiles(extensionDescription, extension, false, out var selectedFiles))
            {
                selectedFile = selectedFiles[0];
                return true;
            }

            selectedFile = null;
            return false;
        }

        private bool OpenFiles(string extensionDescription, string extension, bool multiselect, out string[] selectedFiles)
        {
            _openFileDialog.Reset();
            _openFileDialog.Filter = extensionDescription;
            _openFileDialog.DefaultExt = extension;

            _openFileDialog.Multiselect = multiselect;

            var result = _openFileDialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                selectedFiles = _openFileDialog.FileNames;
                return true;
            }

            selectedFiles = null;
            return false;
        }

        public bool SaveFile(string extensionDescription, string extension, out string filePath)
        {
            _saveFileDialog.Reset();
            _saveFileDialog.Filter = extensionDescription;
            _saveFileDialog.DefaultExt = extension;

            _saveFileDialog.OverwritePrompt = true;
            _saveFileDialog.AddExtension = true;
        
            var dialogResult = _saveFileDialog.ShowDialog();
            if (dialogResult.HasValue && dialogResult.Value)
            {
                filePath = _saveFileDialog.FileNames[0];
                return true;
            }

            filePath = null;
            return false;
        }
    }
}
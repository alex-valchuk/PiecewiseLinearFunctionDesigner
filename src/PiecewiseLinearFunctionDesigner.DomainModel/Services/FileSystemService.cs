using Microsoft.Win32;

namespace PiecewiseLinearFunctionDesigner.DomainModel.Services
{
    public interface IFileSystemService
    {
        bool OpenFile(out string selectedFile);

        bool OpenFiles(bool multiselect, out string[] selectedFiles);
    }

    public class FileSystemService : IFileSystemService
    {
        private readonly OpenFileDialog _openFileDialog = new OpenFileDialog();

        public bool OpenFile(out string selectedFile)
        {
            if (OpenFiles(false, out var selectedFiles))
            {
                selectedFile = selectedFiles[0];
                return true;
            }

            selectedFile = null;
            return false;
        }

        public bool OpenFiles(bool multiselect, out string[] selectedFiles)
        {
            _openFileDialog.Multiselect = true;
            
            var result = _openFileDialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                selectedFiles = _openFileDialog.FileNames;
                return true;
            }

            selectedFiles = null;
            return false;
        }
    }
}
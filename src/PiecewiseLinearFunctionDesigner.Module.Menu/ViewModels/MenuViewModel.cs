using System;
using System.Threading.Tasks;
using System.Windows;
using PiecewiseLinearFunctionDesigner.Core;
using PiecewiseLinearFunctionDesigner.Core.Events;
using PiecewiseLinearFunctionDesigner.DomainModel.Exceptions;
using PiecewiseLinearFunctionDesigner.DomainModel.Services;
using PiecewiseLinearFunctionDesigner.Localization;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;

namespace PiecewiseLinearFunctionDesigner.Module.Menu.ViewModels
{
    public class MenuViewModel : BindableBase
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IFileSystemService _fileSystemService;
        private readonly IProjectService _projectService;
        private readonly IMessageService _messageService;
        private readonly ITextLocalization _textLocalization;
        
        private string _filePath;

        private Visibility _saveVisibility = Visibility.Collapsed;
        public Visibility SaveVisibility
        {
            get { return _saveVisibility; }
            set
            {
                SetProperty(ref _saveVisibility, value);
            }
        }

        private bool _isSaveEnabled;
        public bool IsSaveEnabled
        {
            get { return _isSaveEnabled; }
            set
            {
                SetProperty(ref _isSaveEnabled, value);
                SaveCommand.RaiseCanExecuteChanged();
            }
        }
        
        public DelegateCommand NewCommand { get; }
        
        public DelegateCommand OpenCommand { get; }
        
        public DelegateCommand SaveCommand { get; }
        
        public DelegateCommand ExitCommand { get; }

        public MenuViewModel(
            IEventAggregator eventAggregator,
            IFileSystemService fileSystemService,
            IProjectService projectService,
            IMessageService messageService,
            ITextLocalization textLocalization)
        {
            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
            _fileSystemService = fileSystemService ?? throw new ArgumentNullException(nameof(fileSystemService));
            _projectService = projectService ?? throw new ArgumentNullException(nameof(projectService));
            _messageService = messageService ?? throw new ArgumentNullException(nameof(messageService));
            _textLocalization = textLocalization ?? throw new ArgumentNullException(nameof(textLocalization));

            NewCommand = new DelegateCommand(ExecuteNewCommand);
            OpenCommand = new DelegateCommand(ExecuteOpenCommand);
            SaveCommand = new DelegateCommand(ExecuteSaveCommand, CanExecuteSaveCommand);
            ExitCommand = new DelegateCommand(ExecuteExitCommand);

            _eventAggregator.GetEvent<AnyChangeMadeEvent>().Subscribe(AnyChangeMadeMessageReceived);
        }

        private void ExecuteNewCommand()
        {
            if (IsSaveEnabled)
            {
                if (!_messageService.ActionConfirmed(_textLocalization.UnsavedChanges, _textLocalization.DoYouWannaSaveChangesBeforeExit))
                    return;
            }

            _projectService.AddNewProject();
            _eventAggregator.GetEvent<ProjectSpecifiedEvent>().Publish();
                
            SaveVisibility = Visibility.Visible;
            IsSaveEnabled = true;
        }

        private async void ExecuteOpenCommand()
        {
            if (_fileSystemService.OpenFile(out _filePath))
            {
                try
                {
                    await _projectService.SetActiveProjectAsync(_filePath);
                    _eventAggregator.GetEvent<ProjectSpecifiedEvent>().Publish();

                    SaveVisibility = Visibility.Collapsed;
                }
                catch (InvalidFileTypeException)
                {
                    _messageService.ShowMessage(_textLocalization.InvalidFileType);
                }
            }
        }

        private async void ExecuteSaveCommand()
        {
            await SaveProjectAsync();
        }

        private async Task SaveProjectAsync(bool showConfirmation = true)
        {
            if (!string.IsNullOrWhiteSpace(_filePath) || 
                _fileSystemService.OpenFile(out _filePath))
            {
                try
                {
                    await _projectService.SaveActiveProjectAsync(_filePath);

                    if (showConfirmation)
                    {
                        _messageService.ShowMessage(_textLocalization.ProjectSuccessfullySaved);
                    }
                    
                    IsSaveEnabled = false;
                }
                catch (InvalidFileTypeException)
                {
                    _messageService.ShowMessage(string.Format(_textLocalization.InvalidFileType));
                }
            }
        }

        private bool CanExecuteSaveCommand()
        {
            return SaveVisibility == Visibility.Visible && IsSaveEnabled;
        }

        private async void ExecuteExitCommand()
        {
            if (IsSaveEnabled)
            {
                if (_messageService.ActionConfirmed(_textLocalization.UnsavedChanges, _textLocalization.DoYouWannaSaveChangesBeforeExit))
                {
                    await SaveProjectAsync(false);
                }
            }
            
            Application.Current.Shutdown();
        }

        private void AnyChangeMadeMessageReceived()
        {
            IsSaveEnabled = true;
        }
    }
}
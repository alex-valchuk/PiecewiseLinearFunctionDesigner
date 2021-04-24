using System;
using System.Threading.Tasks;
using System.Windows;
using PiecewiseLinearFunctionDesigner.Core;
using PiecewiseLinearFunctionDesigner.Core.Const;
using PiecewiseLinearFunctionDesigner.Core.Events;
using PiecewiseLinearFunctionDesigner.DomainModel.Exceptions;
using PiecewiseLinearFunctionDesigner.DomainModel.Models;
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
                SaveAndCloseCommand.RaiseCanExecuteChanged();
            }
        }
        
        public DelegateCommand NewCommand { get; private set; }
        
        public DelegateCommand OpenCommand { get; private set; }
        
        public DelegateCommand SaveCommand { get; private set; }
        
        public DelegateCommand SaveAndCloseCommand { get; private set; }
        
        public DelegateCommand ExitCommand { get; private set; }

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

            _eventAggregator.GetEvent<MessageSentEvent>().Subscribe(AnyChangeMadeMessageReceived, ThreadOption.PublisherThread, false, filter => filter.Contains(MessageMarkers.AnyChangeMade));

            NewCommand = new DelegateCommand(ExecuteNewCommand);
            OpenCommand = new DelegateCommand(ExecuteOpenCommand);
            SaveCommand = new DelegateCommand(ExecuteSaveCommand, CanExecuteSaveCommand);
            SaveAndCloseCommand = new DelegateCommand(ExecuteSaveAndCloseCommand, CanExecuteSaveCommand);
            ExitCommand = new DelegateCommand(ExecuteExitCommand);
        }

        private void AnyChangeMadeMessageReceived(string message)
        {
            IsSaveEnabled = true;
        }

        private void ExecuteNewCommand()
        {
            if (IsSaveEnabled)
            {
                if (!_messageService.ActionConfirmed(_textLocalization.UnsavedChanges, _textLocalization.AreYouSureYouWantToCloseActiveProject))
                    return;
            }

            _projectService.SetActiveProject(new Project());
            _eventAggregator.GetEvent<ProjectSpecifiedEvent>().Publish();
                
            SaveVisibility = Visibility.Visible;
            IsSaveEnabled = true;
        }

        private async void ExecuteOpenCommand()
        {
            if (_fileSystemService.OpenFile(out var selectedFile))
            {
                try
                {
                    await _projectService.SetActiveProjectAsync(selectedFile);
                    _eventAggregator.GetEvent<ProjectSpecifiedEvent>().Publish();

                    SaveVisibility = Visibility.Visible;
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

        private async Task<bool> SaveProjectAsync()
        {
            if (_fileSystemService.OpenFile(out var filePath))
            {
                try
                {
                    await _projectService.SaveActiveProjectAsync(filePath);
                    _messageService.ShowMessage(_textLocalization.ProjectSuccessfullySaved);
                    IsSaveEnabled = false;
                    return true;
                }
                catch (InvalidFileTypeException)
                {
                    _messageService.ShowMessage(string.Format(_textLocalization.InvalidFileType));
                }
            }

            return false;
        }

        private bool CanExecuteSaveCommand()
        {
            return SaveVisibility == Visibility.Visible && IsSaveEnabled;
        }

        private async void ExecuteSaveAndCloseCommand()
        {
            if (await SaveProjectAsync())
            {
                _projectService.SetActiveProject(null);
                _eventAggregator.GetEvent<ProjectClosedEvent>().Publish();
                SaveVisibility = Visibility.Collapsed;
            }
        }

        private void ExecuteExitCommand()
        {
            if (IsSaveEnabled)
            {
                if (_messageService.ActionConfirmed(_textLocalization.UnsavedChanges, _textLocalization.AreYouSureYouWantToCloseActiveProject))
                {
                    SaveVisibility = Visibility.Collapsed;
                }
            }
            else
            {
                SaveVisibility = Visibility.Collapsed;
            }
        }
    }
}
using System;
using System.Windows;
using PiecewiseLinearFunctionDesigner.Core;
using PiecewiseLinearFunctionDesigner.Core.Const;
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
                SaveAndExitCommand.RaiseCanExecuteChanged();
            }
        }
        
        public DelegateCommand NewCommand { get; private set; }
        
        public DelegateCommand OpenCommand { get; private set; }
        
        public DelegateCommand SaveCommand { get; private set; }
        
        public DelegateCommand SaveAndExitCommand { get; private set; }
        
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
            SaveAndExitCommand = new DelegateCommand(ExecuteSaveAndExitCommand, CanExecuteSaveCommand);
            ExitCommand = new DelegateCommand(ExecuteExitCommand);
        }

        private void AnyChangeMadeMessageReceived(string message)
        {
            IsSaveEnabled = true;
        }

        private void ExecuteNewCommand()
        {
            SaveVisibility = Visibility.Visible;
        }

        private void ExecuteOpenCommand()
        {
            if (_fileSystemService.OpenFile(out var selectedFile))
            {
                try
                {
                    _projectService.SetActiveProjectFilePath(selectedFile);
                    _eventAggregator.GetEvent<ProjectSpecifiedEvent>().Publish();

                    SaveVisibility = Visibility.Visible;
                }
                catch (InvalidFileTypeException ex)
                {
                    _messageService.ShowMessage(string.Format(_textLocalization.InvalidFileType, ex.SupportedFileTypes));
                }
            }
        }

        private void ExecuteSaveCommand()
        {
            IsSaveEnabled = false;
        }

        private bool CanExecuteSaveCommand()
        {
            return SaveVisibility == Visibility.Visible && IsSaveEnabled;
        }

        private void ExecuteSaveAndExitCommand()
        {
            IsSaveEnabled = false;
        }

        private void ExecuteExitCommand()
        {
            if (IsSaveEnabled)
            {
                if (_messageService.ActionConfirmed("Есть несохраненные изменения.", "Уверены, что хотите закрыть?"))
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
using System;
using System.Threading;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Threading;
using Packlists.Model.ProgressBar;

namespace Packlists.ViewModel
{
    public class ProgressDialogViewModel : ViewModelBase 
    {
        #region Privates

        string _windowTitle;
        string _label;
        string _subLabel;
        private int _currentTaskNumber;
        private int _maxTaskNumber;
        private bool _isIndeterminate;
        bool _close;

        #endregion

        #region Publics

        public string WindowTitle
        {
            get => _windowTitle;
            private set => Set(nameof(WindowTitle), ref _windowTitle, value);
        }

        public string Label
        {
            get => _label;
            private set => Set(nameof(Label), ref _label, value);
        }

        public string SubLabel
        {
            get => _subLabel;
            private set => Set(nameof(SubLabel), ref _subLabel, value);
        }

        public bool Close
        {
            get => _close;
            set => Set(nameof(Close), ref _close, value);
        }

        public bool IsCancellable => CancelCommand != null;
        public CancelCommand CancelCommand { get; private set; }
        public IProgress<ProgressReport> Progress { get; private set; }

        public bool IsIndeterminate
        {
            get => _isIndeterminate;
            set
            {
                _isIndeterminate = value;
                RaisePropertyChanged(nameof(IsIndeterminate));
            }
        }

        public int MaxTaskNumber
        {
            get => _maxTaskNumber;
            set
            {
                _maxTaskNumber = value; 
                RaisePropertyChanged(nameof(MaxTaskNumber));
            }
        }

        public int CurrentTaskNumber
        {
            get => _currentTaskNumber;
            set
            {
                _currentTaskNumber = value; 
                RaisePropertyChanged(nameof(CurrentTaskNumber));
            }
        }

        #endregion

        public ProgressDialogViewModel
        (
            ProgressDialogOptions options,
            CancellationToken cancellationToken,
            CancelCommand cancelCommand
        )
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            WindowTitle = options.WindowTitle;
            Label = options.Label;
            CancelCommand = cancelCommand; // can be null (not cancellable)
            cancellationToken.Register(OnCancelled);
            Progress = new Progress<ProgressReport>(OnProgress);
        }

        void OnCancelled()
        {
            // Cancellation may come from a background thread.
            if (DispatcherHelper.UIDispatcher != null)
                DispatcherHelper.CheckBeginInvokeOnUI(() => Close = true);
            else
                Close = true;
        }

        void OnProgress(ProgressReport obj)
        {
            // Progress will probably come from a background thread.
            if (DispatcherHelper.UIDispatcher != null)
                DispatcherHelper.CheckBeginInvokeOnUI(() => OnProgressReceived(obj));
            else
                OnProgressReceived(obj);

        }

        private void OnProgressReceived(ProgressReport progressReport)
        {
            if (progressReport.IsIndeterminate)
            {
                IsIndeterminate = progressReport.IsIndeterminate;
                SubLabel = progressReport.CurrentTask;
                return;
            }
            SubLabel = progressReport.CurrentTask;
            CurrentTaskNumber = progressReport.CurrentTaskNumber;
            MaxTaskNumber = progressReport.MaxTaskNumber;
            IsIndeterminate = progressReport.IsIndeterminate;
        }

    }
}

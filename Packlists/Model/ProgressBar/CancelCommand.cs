using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Packlists.Model.ProgressBar
{
    public class CancelCommand : ICommand
    {
        readonly CancellationTokenSource cancellationTokenSource;

        public event EventHandler CanExecuteChanged;

        public CancelCommand(CancellationTokenSource cancellationTokenSource)
        {
            this.cancellationTokenSource = cancellationTokenSource ?? throw new ArgumentNullException(nameof(cancellationTokenSource));
        }

        public bool CanExecute(object parameter)
        {
            return !cancellationTokenSource.IsCancellationRequested;
        }

        public void Execute(object parameter)
        {
            cancellationTokenSource.Cancel();

            CanExecuteChanged?.Invoke(this, EventArgs.Empty);

            CommandManager.InvalidateRequerySuggested();
        }
    }
}

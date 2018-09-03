using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Packlists.ViewModel;

namespace Packlists.Model.ProgressBar
{
    public class ProgressDialogService : IProgressDialogService
    {
        private readonly TaskFactory _taskFactory;

        public ProgressDialogService() : this(Task.Factory) { }

        private ProgressDialogService(TaskFactory taskFactory)
        {
            this._taskFactory = taskFactory ?? throw new ArgumentNullException(nameof(taskFactory));
        }

        public void Execute(Action action, ProgressDialogOptions options)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            ExecuteInternal((token, progress) => action(), options,
                isCancellable: false);
        }

        public void Execute(Action<CancellationToken> action, ProgressDialogOptions options)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            ExecuteInternal((token, progress) => action(token), options);
        }

        public void Execute(Action<IProgress<ProgressReport>> action, ProgressDialogOptions options)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            ExecuteInternal((token, progress) => action(progress), options);
        }

        public void Execute(
            Action<CancellationToken, IProgress<ProgressReport>> action, ProgressDialogOptions options)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            ExecuteInternal(action, options);
        }

        public bool TryExecute<T>(Func<T> action, ProgressDialogOptions options, out T result)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            return TryExecuteInternal((token, progress) => action(), options, out result,
                isCancellable: false);
        }

        public bool TryExecute<T>(Func<CancellationToken, T> action, ProgressDialogOptions options, out T result)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            return TryExecuteInternal((token, progress) => action(token), options, out result);
        }

        public bool TryExecute<T>(Func<IProgress<ProgressReport>, T> action, ProgressDialogOptions options, out T result)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            return TryExecuteInternal((token, progress) => action(progress), options, out result,
                isCancellable: false);
        }

        public bool TryExecute<T>(Func<CancellationToken, IProgress<ProgressReport>, T> action,
            ProgressDialogOptions options, out T result)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            return TryExecuteInternal(action, options, out result);
        }

        public async Task ExecuteAsync(Func<Task> action, ProgressDialogOptions options)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            await ExecuteAsyncInternal((token, progress) => action(), options);
        }

        public async Task ExecuteAsync(Func<CancellationToken, Task> action, ProgressDialogOptions options)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            await ExecuteAsyncInternal((token, progress) => action(token), options);
        }

        public async Task ExecuteAsync(Func<IProgress<ProgressReport>, Task> action, ProgressDialogOptions options)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            await ExecuteAsyncInternal((token, progress) => action(progress), options,
                isCancellable: false);
        }

        public async Task ExecuteAsync(Func<CancellationToken, IProgress<ProgressReport>, Task> action,
            ProgressDialogOptions options)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            await ExecuteAsyncInternal(action, options);
        }

        public async Task<T> ExecuteAsync<T>(Func<Task<T>> action, ProgressDialogOptions options)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            return await ExecuteAsyncInternal((token, progress) => action(), options);
        }

        public async Task<T> ExecuteAsync<T>(Func<CancellationToken, Task<T>> action, ProgressDialogOptions options)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            return await ExecuteAsyncInternal((token, progress) => action(token), options);
        }

        public async Task<T> ExecuteAsync<T>(Func<IProgress<ProgressReport>, Task<T>> action, ProgressDialogOptions options)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            return await ExecuteAsyncInternal((token, progress) => action(progress), options,
                isCancellable: false);
        }

        public async Task<T> ExecuteAsync<T>(Func<CancellationToken, IProgress<ProgressReport>, Task<T>> action,
            ProgressDialogOptions options)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            return await ExecuteAsyncInternal(action, options);
        }

        private void ExecuteInternal(Action<CancellationToken, IProgress<ProgressReport>> action,
            ProgressDialogOptions options, bool isCancellable = true)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            if (options == null) throw new ArgumentNullException(nameof(options));

            using (var cancellationTokenSource = new CancellationTokenSource())
            {
                CancellationToken cancellationToken = cancellationTokenSource.Token;

                var cancelCommand = isCancellable ? new CancelCommand(cancellationTokenSource) : null;

                var viewModel = new ProgressDialogViewModel(
                    options, cancellationToken, cancelCommand);

                var window = new ProgressDialog
                                 {
                                     DataContext = viewModel,
                                 };

                var task = _taskFactory
                    .StartNew(() => action(cancellationToken, viewModel.Progress),
                              cancellationToken);

                task.ContinueWith(_ => viewModel.Close = true);

                window.ShowDialog();
            }
        }

        private bool TryExecuteInternal<T>(
            Func<CancellationToken, IProgress<ProgressReport>, T> action,
            ProgressDialogOptions options, out T result, bool isCancellable = true)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            if (options == null) throw new ArgumentNullException(nameof(options));

            using (var cancellationTokenSource = new CancellationTokenSource())
            {
                CancellationToken cancellationToken = cancellationTokenSource.Token;

                var cancelCommand = isCancellable ? new CancelCommand(cancellationTokenSource) : null;

                var viewModel = new ProgressDialogViewModel(
                    options, cancellationToken, cancelCommand);

                var window = new ProgressDialog
                {
                    DataContext = viewModel
                };

                var task = _taskFactory
                    .StartNew(() => action(cancellationToken, viewModel.Progress),
                              cancellationToken);

                task.ContinueWith(_ => viewModel.Close = true);

                window.ShowDialog();

                if (task.IsCanceled)
                {
                    result = default(T);
                    return false;
                }

                if (task.IsCompleted)
                {
                    result = task.Result;
                    return true;
                }

                result = default(T);
                return false;
            }
        }

        async private Task<T> ExecuteAsyncInternal<T>(
            Func<CancellationToken, IProgress<ProgressReport>, Task<T>> action,
            ProgressDialogOptions options, bool isCancellable = true)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            if (options == null) throw new ArgumentNullException(nameof(options));

            using (var cancellationTokenSource = new CancellationTokenSource())
            {
                CancellationToken cancellationToken = cancellationTokenSource.Token;

                var cancelCommand = isCancellable ? new CancelCommand(cancellationTokenSource) : null;

                var viewModel = new ProgressDialogViewModel(
                    options, cancellationToken, cancelCommand);

                var window = new ProgressDialog
                {
                    DataContext = viewModel
                };

                Task<T> task = action(cancellationToken, viewModel.Progress);

                task.ContinueWith(_ => viewModel.Close = true);

                window.ShowDialog();

                return await task;
            }
        }

        async private Task ExecuteAsyncInternal(
            Func<CancellationToken, IProgress<ProgressReport>, Task> action,
            ProgressDialogOptions options, bool isCancellable = true)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            if (options == null) throw new ArgumentNullException(nameof(options));

            using (var cancellationTokenSource = new CancellationTokenSource())
            {
                CancellationToken cancellationToken = cancellationTokenSource.Token;

                var cancelCommand = isCancellable ? new CancelCommand(cancellationTokenSource) : null;

                var viewModel = new ProgressDialogViewModel(
                    options, cancellationToken, cancelCommand);

                var window = new ProgressDialog
                {
                    DataContext = viewModel
                };

                Task task = action(cancellationToken, viewModel.Progress);

                task.ContinueWith(_ => viewModel.Close = true);

                window.ShowDialog();

                await task;
            }
        }
    }
}

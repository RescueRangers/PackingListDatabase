using System;
using System.Threading;
using System.Threading.Tasks;

namespace Packlists.Model.ProgressBar
{
    public interface IProgressDialogService
    {
        /// <summary>
        /// Executes a non-cancellable task.
        /// </summary>
        void Execute(Action action, ProgressDialogOptions options);

        /// <summary>
        /// Executes a cancellable task.
        /// </summary>
        void Execute(Action<CancellationToken> action, ProgressDialogOptions options);

        /// <summary>
        /// Executes a non-cancellable task that reports progress.
        /// </summary>
        void Execute(Action<IProgress<ProgressReport>> action, ProgressDialogOptions options);

        /// <summary>
        /// Executes a cancellable task that reports progress.
        /// </summary>
        void Execute(Action<CancellationToken, IProgress<ProgressReport>> action,
                     ProgressDialogOptions options);

        /// <summary>
        /// Executes a non-cancellable task, that returns a value.
        /// </summary>
        bool TryExecute<T>(Func<T> action, ProgressDialogOptions options,
                           out T result);

        /// <summary>
        /// Executes a cancellable task that returns a value.
        /// </summary>
        bool TryExecute<T>(Func<CancellationToken, T> action,
                           ProgressDialogOptions options, out T result);

        /// <summary>
        /// Executes a non-cancellable task that reports progress and returns a value.
        /// </summary>
        bool TryExecute<T>(Func<IProgress<ProgressReport>, T> action,
                           ProgressDialogOptions options, out T result);

        /// <summary>
        /// Executes a cancellable task that reports progress and returns a value.
        /// </summary>
        bool TryExecute<T>(Func<CancellationToken, IProgress<ProgressReport>, T> action,
                           ProgressDialogOptions options, out T result);

        /// <summary>
        /// Executes a non-cancellable async task.
        /// </summary>
        Task ExecuteAsync(Func<Task> action, ProgressDialogOptions options);

        /// <summary>
        /// Executes a cancellable async task.
        /// </summary>
        Task ExecuteAsync(Func<CancellationToken, Task> action, ProgressDialogOptions options);

        /// <summary>
        /// Executes a non-cancellable async task that reports progress.
        /// </summary>
        Task ExecuteAsync(Func<IProgress<ProgressReport>, Task> action, ProgressDialogOptions options);

        /// <summary>
        /// Executes a cancellable async task that reports progress.
        /// </summary>
        Task ExecuteAsync(Func<CancellationToken, IProgress<ProgressReport>, Task> action,
                          ProgressDialogOptions options);

        /// <summary>
        /// Executes a non-cancellable async task that returns a value.
        /// </summary>
        Task<T> ExecuteAsync<T>(Func<Task<T>> action, ProgressDialogOptions options);

        /// <summary>
        /// Executes a cancellable async task that returns a value.
        /// </summary>
        Task<T> ExecuteAsync<T>(Func<CancellationToken, Task<T>> action, ProgressDialogOptions options);

        /// <summary>
        /// Executes a non-cancellable async task that reports progress and returns a value.
        /// </summary>
        Task<T> ExecuteAsync<T>(Func<IProgress<ProgressReport>, Task<T>> action, ProgressDialogOptions options);

        /// <summary>
        /// Executes a cancellable async task that reports progress and returns a value.
        /// </summary>
        Task<T> ExecuteAsync<T>(Func<CancellationToken, IProgress<ProgressReport>, Task<T>> action,
                                ProgressDialogOptions options);
    }
}
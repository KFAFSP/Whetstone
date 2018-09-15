using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

using JetBrains.Annotations;

namespace Whetstone.Threading
{
    /// <summary>
    /// Provides some extension methods for use with the <see cref="IAwaitable"/> and
    /// <see cref="IAwaitable{TResult}"/> types.
    /// </summary>
    [PublicAPI]
    [ExcludeFromCodeCoverage]
    public static class AwaitableExtensions
    {
        /// <summary>
        /// Synchronously wait for the event.
        /// </summary>
        /// <param name="AAwaitable">The <see cref="IAwaitable"/>.</param>
        public static void Wait([NotNull] this IAwaitable AAwaitable)
            => AAwaitable.Wait(CancellationToken.None);

        /// <summary>
        /// Synchronously wait for the event.
        /// </summary>
        /// <param name="AAwaitable">The <see cref="IAwaitable"/>.</param>
        /// <param name="ACancel">A <see cref="CancellationToken"/> to cancel the wait.</param>
        /// <exception cref="OperationCanceledException">The wait was canceled.</exception>
        /// <exception cref="ObjectDisposedException">
        /// The <see cref="CancellationTokenSource"/> of <paramref name="ACancel"/> is disposed.
        /// </exception>
        /// <remarks>
        /// If required, internal exceptions that were wrapped in a <see cref="AggregateException"/>
        /// are unwrapped and rethrown.
        /// </remarks>
        public static void Wait([NotNull] this IAwaitable AAwaitable, CancellationToken ACancel)
        {
            try
            {
                AAwaitable.WaitAsync(ACancel).Wait(ACancel);
            }
            catch (AggregateException error)
            {
                throw error.InnerException ?? error;
            }
        }

        /// <summary>
        /// Wait for the event.
        /// </summary>
        /// <param name="AAwaitable">The <see cref="IAwaitable"/>.</param>
        /// <returns>An awaitable <see cref="Task"/> that waits for the result.</returns>
        [NotNull]
        public static Task WaitAsync([NotNull] this IAwaitable AAwaitable)
            => AAwaitable.WaitAsync(CancellationToken.None);

        /// <summary>
        /// Synchronously wait for the result.
        /// </summary>
        /// <typeparam name="TResult">The awaitable result type.</typeparam>
        /// <param name="AAwaitable">The <see cref="IAwaitable{TResult}"/>.</param>
        public static TResult Wait<TResult>([NotNull] this IAwaitable<TResult> AAwaitable)
            => AAwaitable.Wait(CancellationToken.None);

        /// <summary>
        /// Synchronously wait for the result.
        /// </summary>
        /// <typeparam name="TResult">The awaitable result type.</typeparam>
        /// <param name="AAwaitable">The <see cref="IAwaitable{TResult}"/>.</param>
        /// <param name="ACancel">A <see cref="CancellationToken"/> to cancel the wait.</param>
        /// <exception cref="OperationCanceledException">The wait was canceled.</exception>
        /// <exception cref="ObjectDisposedException">
        /// The <see cref="CancellationTokenSource"/> of <paramref name="ACancel"/> is disposed.
        /// </exception>
        /// <remarks>
        /// If required, internal exceptions that were wrapped in a <see cref="AggregateException"/>
        /// are unwrapped and rethrown.
        /// </remarks>
        public static TResult Wait<TResult>(
            [NotNull] this IAwaitable<TResult> AAwaitable,
            CancellationToken ACancel
        )
        {
            try
            {
                var task = AAwaitable.WaitAsync(ACancel);
                task.Wait(ACancel);
                return task.Result;
            }
            catch (AggregateException error)
            {
                throw error.InnerException ?? error;
            }
        }

        /// <summary>
        /// Wait for the result.
        /// </summary>
        /// <typeparam name="TResult">The awaitable result type.</typeparam>
        /// <param name="AAwaitable">The <see cref="IAwaitable{TResult}"/>.</param>
        /// <returns>An awaitable <see cref="Task{TResult}"/> that waits for the result.</returns>
        [NotNull]
        public static Task<TResult> WaitAsync<TResult>(
            [NotNull] this IAwaitable<TResult> AAwaitable
        ) => AAwaitable.WaitAsync(CancellationToken.None);

        /// <summary>
        /// Get an awaiter for this <see cref="IAwaitable"/>.
        /// </summary>
        /// <param name="AAwaitable">The <see cref="IAwaitable"/>.</param>
        /// <returns>An awaiter for <paramref name="AAwaitable"/>.</returns>
        public static TaskAwaiter GetAwaiter([NotNull] this IAwaitable AAwaitable)
            => AAwaitable.WaitAsync().GetAwaiter();

        /// <summary>
        /// Get an awaiter for this <see cref="IAwaitable{TResult}"/>.
        /// </summary>
        /// <typeparam name="TResult">The awaitable result type.</typeparam>
        /// <param name="AAwaitable">The <see cref="IAwaitable{TResult}"/>.</param>
        /// <returns>An awaiter for <paramref name="AAwaitable"/>.</returns>
        public static TaskAwaiter<TResult> GetAwaiter<TResult>(
            [NotNull] this IAwaitable<TResult> AAwaitable
        ) => AAwaitable.WaitAsync().GetAwaiter();
    }
}
using System;
using System.Diagnostics.CodeAnalysis;
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
        public static void Wait([NotNull] this IAwaitable AAwaitable, CancellationToken ACancel)
            => AAwaitable.WaitAsync(ACancel).Wait(ACancel);

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
        public static void Wait<TResult>([NotNull] this IAwaitable<TResult> AAwaitable)
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
        public static void Wait<TResult>(
            [NotNull] this IAwaitable<TResult> AAwaitable,
            CancellationToken ACancel
        ) => AAwaitable.WaitAsync(ACancel).Wait(ACancel);

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
    }
}
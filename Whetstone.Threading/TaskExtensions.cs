using System.Threading;
using System.Threading.Tasks;

using JetBrains.Annotations;

namespace Whetstone.Threading
{
    /// <summary>
    /// Provides some extension methods for use with the <see cref="Task"/> and
    /// <see cref="Task{TResult}"/> types.
    /// </summary>
    [PublicAPI]
    public static class TaskExtensions
    {
        /// <summary>
        /// Wrap a <see cref="Task"/> with an identical <see cref="Task"/> that introduces a new
        /// cancellation mode.
        /// </summary>
        /// <param name="ATask">The <see cref="Task"/>.</param>
        /// <param name="ACancel">A new <see cref="CancellationToken"/> to consider.</param>
        /// <returns>
        /// An awaitable <see cref="Task"/> that mimics <paramref name="ATask"/> but extends it with
        /// the new cancellation mode of <paramref name="ACancel"/>.
        /// </returns>
        /// <remarks>
        /// Signalling <paramref name="ACancel"/> will cancel the resulting task but not
        /// <paramref name="ATask"/>!
        /// </remarks>
        [NotNull]
        public static Task OrCancelledBy(
            [NotNull] this Task ATask,
            CancellationToken ACancel
        )
        {
            // Quick return: the task is completed or the new cancellation mode is useless.
            if (ATask.IsCompleted || !ACancel.CanBeCanceled) return ATask;
            // Quick return: the cancellation was already requested.
            if (ACancel.IsCancellationRequested) return Task.FromCanceled(ACancel);

            var tcs = new TaskCompletionSource<Void>();
            async Task Internal()
            {
                using (ACancel.Register(() => tcs.TrySetCanceled(ACancel)))
                {
                    await Task.WhenAny(ATask, tcs.Task);
                }
            }

            return Internal();
        }

        /// <summary>
        /// Wrap a <see cref="Task{TResult}"/> with an identical <see cref="Task{TResult}"/> that
        /// introduces a new cancellation mode.
        /// </summary>
        /// <param name="ATask">The <see cref="Task{TResult}"/>.</param>
        /// <param name="ACancel">A new <see cref="CancellationToken"/> to consider.</param>
        /// <returns>
        /// An awaitable <see cref="Task{TResult}"/> that mimics <paramref name="ATask"/> but
        /// extends it with the new cancellation mode of <paramref name="ACancel"/>.
        /// </returns>
        /// <remarks>
        /// Signalling <paramref name="ACancel"/> will cancel the resulting task but not
        /// <paramref name="ATask"/>!
        /// </remarks>
        [NotNull]
        public static Task<TResult> OrCancelledBy<TResult>(
            [NotNull] this Task<TResult> ATask,
            CancellationToken ACancel
        )
        {
            // Quick return: the task is completed or the new cancellation mode is useless.
            if (ATask.IsCompleted || !ACancel.CanBeCanceled) return ATask;
            // Quick return: the cancellation was already requested.
            if (ACancel.IsCancellationRequested) return Task.FromCanceled<TResult>(ACancel);

            var tcs = new TaskCompletionSource<TResult>();

            async Task<TResult> Internal()
            {
                using (ACancel.Register(() => tcs.TrySetCanceled(ACancel)))
                {
                    return await Task.WhenAny(ATask, tcs.Task).Result;
                }
            }

            return Internal();
        }
    }
}
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using JetBrains.Annotations;

using Whetstone.Contracts;

namespace Whetstone.Threading
{
    /// <summary>
    /// Represents an era that starts with it's creation and can be ended once.
    /// </summary>
    [PublicAPI]
    public sealed class Era : Disposable, IAwaitable
    {
        /// <summary>
        /// Get a new <see cref="Era"/> that has ended.
        /// </summary>
        [NotNull]
        public static Era Ended
        {
            get
            {
                var era = new Era();
                var ok = era.TryEnd();
                Debug.Assert(
                    ok,
                    "Era contested.",
                    "This indicates a severe runtime problem."
                );
                return era;
            }
        }

        readonly TaskCompletionSource<Void> FSource = new TaskCompletionSource<Void>();

        #region Disposable overrides
        /// <inheritdoc />
        protected override void Dispose(bool ADisposing)
        {
            if (ADisposing)
            {
                // If the era did not end yet, fault it.
                FSource.TrySetException(new ObjectDisposedException(typeof(Era).Name));
            }

            base.Dispose(ADisposing);
        }
        #endregion

        /// <summary>
        /// Try to end this era.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if the era ended; <see langword="false"/> if it had already
        /// ended.
        /// </returns>
        /// <remarks>
        /// This method is thread-safe. When called concurrently, only the first caller will receive
        /// a positive result from the call.
        /// </remarks>
        public bool TryEnd() => FSource.TrySetResult(default);

        #region IAwaitable
        /// <inheritdoc />
        public Task WaitAsync(CancellationToken ACancel) => FSource.Task.OrCancelledBy(ACancel);
        #endregion

        /// <summary>
        /// Get a value indicating whether this era has ended.
        /// </summary>
        public bool HasEnded => FSource.Task.IsCompleted;
    }
}

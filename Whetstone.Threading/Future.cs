using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using JetBrains.Annotations;

using Whetstone.Contracts;

namespace Whetstone.Threading
{
    /// <summary>
    /// Provides static factory methods for the <see cref="Future"/> class.
    /// </summary>
    [PublicAPI]
    public static class Future
    {
        /// <summary>
        /// Create a new <see cref="Future{TValue}"/> with an existing value.
        /// </summary>
        /// <param name="AValue">The value.</param>
        /// <returns>A new <see cref="Future{TValue}"/> with an existing value.</returns>
        [NotNull]
        public static Future<TValue> Of<TValue>(TValue AValue)
        {
            var future = new Future<TValue>();
            var ok = future.TryPost(AValue);
            Debug.Assert(
                ok,
                "Future contested.",
                "This indicates a severe runtime problem."
            );
            return future;
        }
    }

    /// <summary>
    /// Represents a future value that is eventually provided.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <remarks>
    /// Essentially an <see cref="Era"/> that also produces a result value.
    /// </remarks>
    [PublicAPI]
    public sealed class Future<TValue> : Disposable, IAwaitable<TValue>
    {
        [NotNull]
        readonly TaskCompletionSource<TValue> FSource = new TaskCompletionSource<TValue>();

        #region Disposable overrides
        /// <inheritdoc />
        protected override void Dispose(bool ADisposing)
        {
            if (ADisposing)
            {
                // If the future does not exist yet, fault it.
                FSource.TrySetException(new ObjectDisposedException(typeof(Era).Name));
            }

            base.Dispose(ADisposing);
        }
        #endregion

        /// <summary>
        /// Try to post a value to this future.
        /// </summary>
        /// <param name="AValue">The value.</param>
        /// <returns>
        /// <see langword="true"/> if the value was posted; <see langword="false"/> if the future
        /// already exists.
        /// </returns>
        public bool TryPost(TValue AValue) => FSource.TrySetResult(AValue);

        #region IAwaitable<TValue>
        /// <inheritdoc />
        public Task<TValue> WaitAsync(CancellationToken ACancel)
            => FSource.Task.OrCanceledBy(ACancel);
        #endregion

        /// <summary>
        /// Get a value indicating whether the value exists.
        /// </summary>
        public bool Exists => FSource.Task.Status == TaskStatus.RanToCompletion;

        /// <summary>
        /// Get or set the value of this future.
        /// </summary>
        /// <exception cref="ObjectDisposedException">This instance is disposed.</exception>
        /// <exception cref="InvalidOperationException">Future already exists.</exception>
        public TValue Value
        {
            [Pure]
            get => this.Wait();
            set
            {
                if (TryPost(value)) return;

                ThrowIfDisposed();
                throw new InvalidOperationException("Future already exists.");
            }
        }
    }
}
using System.Threading;
using System.Threading.Tasks;

using JetBrains.Annotations;

using Whetstone.Contracts;

namespace Whetstone.Threading
{
    /// <summary>
    /// Represents a source of awaitable events with associated data.
    /// </summary>
    /// <typeparam name="TData">The data type.</typeparam>
    /// <remarks>
    /// Essentially an <see cref="Trigger"/> that also produces a result value.
    /// </remarks>
    [PublicAPI]
    public sealed class Event<TData> : Disposable, IAwaitable<TData>
    {
        // NOTE: Atomically exchanged reference to the current future.
        [NotNull]
        Future<TData> FFuture = new Future<TData>();

        #region Disposable overrides
        /// <inheritdoc />
        protected override void Dispose(bool ADisposing)
        {
            if (ADisposing)
            {
                // Dispose the current event future.
                FFuture.Dispose();
            }

            base.Dispose(ADisposing);
        }
        #endregion

        /// <summary>
        /// Fire an event.
        /// </summary>
        /// <param name="AData">The data.</param>
        public void Fire(TData AData)
        {
            ThrowIfDisposed();

            var old = Interlocked.Exchange(ref FFuture, new Future<TData>());
            old.TryPost(AData);
        }

        #region IAwaitable<TValue>
        /// <inheritdoc />
        public Task<TData> WaitAsync(CancellationToken ACancel) => FFuture.WaitAsync(ACancel);
        #endregion
    }
}

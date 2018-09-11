using System;
using System.Threading;
using System.Threading.Tasks;

using JetBrains.Annotations;

using Whetstone.Contracts;

namespace Whetstone.Threading
{
    /// <summary>
    /// Represents an awaitable trigger that can be fire multiple times.
    /// </summary>
    /// <remarks>
    /// If the events coincide with a produced value, consider using <see cref="Event{TValue}"/>
    /// instead.
    /// </remarks>
    [PublicAPI]
    public sealed class Trigger : Disposable, IAwaitable
    {
        // NOTE: Atomically exchanged reference to the current era.
        [NotNull]
        Era FEra = new Era();

        #region Disposable overrides
        /// <inheritdoc />
        protected override void Dispose(bool ADisposing)
        {
            if (ADisposing)
            {
                // Dispose the current trigger era.
                FEra.Dispose();
            }

            base.Dispose(ADisposing);
        }
        #endregion

        /// <summary>
        /// Fire the trigger once.
        /// </summary>
        /// <exception cref="ObjectDisposedException">This object is disposed.</exception>
        /// <remarks>
        /// A wait that is started during the execution of this method may or may not be singalled
        /// by the current firing.
        /// </remarks>
        public void Fire()
        {
            ThrowIfDisposed();

            var old = Interlocked.Exchange(ref FEra, new Era());
            old.TryEnd();
        }

        #region IAwaitable
        /// <inheritdoc />
        public Task WaitAsync(CancellationToken ACancel) => FEra.WaitAsync(ACancel);
        #endregion
    }
}
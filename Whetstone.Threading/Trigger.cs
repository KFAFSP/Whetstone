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
    [PublicAPI]
    public sealed class Trigger : Disposable, IAwaitable
    {
        // NOTE: Atomically exchanged reference by Fire().
        [NotNull]
        volatile Era FEra = new Era();

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

            if (FEra.TryEnd()) FEra = new Era();
        }

        #region IAwaitable
        /// <inheritdoc />
        public Task WaitAsync(CancellationToken ACancel) => FEra.WaitAsync(ACancel);
        #endregion
    }
}
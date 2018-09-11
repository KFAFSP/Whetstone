using System;
using System.Threading;
using System.Threading.Tasks;

using JetBrains.Annotations;

using Whetstone.Contracts;

namespace Whetstone.Threading
{
    /// <summary>
    /// Represents an awaitable boolean condition.
    /// </summary>
    [PublicAPI]
    public sealed class Condition : Disposable, IAwaitable
    {
        /// <summary>
        /// Get a new <see cref="Condition"/> that is <see langword="true"/>.
        /// </summary>
        [NotNull]
        public static Condition True => new Condition(true);
        /// <summary>
        /// Get a new <see cref="Condition"/> that is <see langword="false"/>.
        /// </summary>
        [NotNull]
        public static Condition False => new Condition();

        int FValue;
        [NotNull]
        readonly Trigger FTrigger = new Trigger();

        /// <summary>
        /// Create a new <see cref="Condition"/>.
        /// </summary>
        /// <param name="AInitialValue">The initial value of the condition.</param>
        public Condition(bool AInitialValue = false)
        {
            FValue = AInitialValue ? 1 : 0;
        }

        #region Disposable overrides
        /// <inheritdoc />
        protected override void Dispose(bool ADisposing)
        {
            if (ADisposing)
            {
                // Dispose of the trigger to fault any outstanding wait.
                FTrigger.Dispose();
            }

            base.Dispose(ADisposing);
        }
        #endregion

        /// <summary>
        /// Try to set the condition to <see langword="true"/>.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if the condition was set; <see langword="false"/> otherwise.
        /// </returns>
        /// <exception cref="ObjectDisposedException">This object is disposed.</exception>
        /// <remarks>
        /// This method is thread-safe. If multiple concurrent calls happen, only the first caller
        /// will receive a positive result.
        /// </remarks>
        public bool TrySet()
        {
            ThrowIfDisposed();

            // Attempt to set the value to "true".
            if (Interlocked.Exchange(ref FValue, 1) != 0) return false;

            // Signal the trigger to unblock all waiting users.
            FTrigger.Fire();
            return true;
        }
        /// <summary>
        /// Try to set the condition to <see langword="false"/>.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if the condition was reset; <see langword="false"/> otherwise.
        /// </returns>
        /// <exception cref="ObjectDisposedException">This object is disposed.</exception>
        /// <remarks>
        /// This method is thread-safe. If multiple concurrent calls happen, only the first caller
        /// will receive a positive result.
        /// </remarks>
        public bool TryReset()
        {
            ThrowIfDisposed();

            // Attempt to set the value to "false".
            return Interlocked.Exchange(ref FValue, 0) == 1;
        }

        #region IAwaitable
        /// <inheritdoc />
        public Task WaitAsync(CancellationToken ACancel)
        {
            // Do not block unless the value is false right now.
            return Value
                ? Task.CompletedTask
                : FTrigger.WaitAsync(ACancel);
        }
        #endregion

        /// <summary>
        /// Get the current value of the condition.
        /// </summary>
        public bool Value
        {
            get => FValue == 1;
            set
            {
                if (value)
                    TrySet();
                else
                    TryReset();
            }
        }

        /// <summary>
        /// Implicitly convert this <see cref="Condition"/> to it's <see cref="Value"/>.
        /// </summary>
        /// <param name="ACondition">The <see cref="Condition"/>.</param>
        public static implicit operator bool([NotNull] Condition ACondition) => ACondition.Value;
    }
}

using System;

using JetBrains.Annotations;

using SC = System.Collections;
using SCG = System.Collections.Generic;

namespace Whetstone.Collections.Enumerators
{
    /// <summary>
    /// Base class for implementing an <see cref="SCG.IEnumerator{T}"/>.
    /// </summary>
    /// <typeparam name="T">The item type.</typeparam>
    [PublicAPI]
    public abstract class EnumeratorBase<T> : SCG.IEnumerator<T>
    {
        private EnumeratorState FState;
        private T FCurrent;

        /// <summary>
        /// Create a new <see cref="EnumeratorBase{T}"/> instance.
        /// </summary>
        protected EnumeratorBase()
        {
            FState = EnumeratorState.BeforeBinding;
        }

        /// <summary>
        /// Bind this enumerator to the data source.
        /// </summary>
        protected abstract void DoBind();
        /// <summary>
        /// Unbind this enumerator from the data source.
        /// </summary>
        protected abstract void DoUnbind();

        /// <summary>
        /// Try to get the next data from the data source.
        /// </summary>
        /// <param name="AValue">The output data.</param>
        /// <returns><c>true</c> if successful; <c>false</c> if exhausted.</returns>
        protected abstract bool DoMoveNext(out T AValue);
        /// <summary>
        /// Reset a bound enumerator.
        /// </summary>
        protected abstract void DoReset();

        /// <summary>
        /// Ensure that the enumerator is bound.
        /// </summary>
        protected void Bind()
        {
            if (IsBound) return;
            if (IsDisposed) throw new InvalidOperationException("Enumerator is disposed.");

            DoBind();
            FState = EnumeratorState.Ready;
        }

        #region IDisposable
        /// <inheritdoc />
        public void Dispose()
        {
            if (IsDisposed) throw new InvalidOperationException("Enumerator is already disposed.");
            if (IsBound) DoUnbind();

            FState = EnumeratorState.Disposed;
        }
        #endregion

        #region IEnumerator
        /// <inheritdoc />
        public bool MoveNext()
        {
            Bind();

            if (IsExhausted)
                return false;

            if (DoMoveNext(out FCurrent))
                return true;

            FState = EnumeratorState.Exhausted;
            return false;
        }

        /// <inheritdoc />
        public void Reset()
        {
            if (!IsBound)
                throw new InvalidOperationException("Enumerator is not bound.");

            DoReset();
        }

        object SC.IEnumerator.Current => Current;
        #endregion

        #region IEnumerator<T>
        /// <inheritdoc />
        public T Current
        {
            get => IsReady ? FCurrent : throw new InvalidOperationException("Enumerator is not bound.");
            protected set => FCurrent = value;
        }
        #endregion

        /// <summary>
        /// Get a value indicating whether this enumerator is bound to the datasource.
        /// </summary>
        public bool IsBound => FState == EnumeratorState.Ready || FState == EnumeratorState.Exhausted;
        /// <summary>
        /// Get a value indicating whether this enumerator is in enumeration.
        /// </summary>
        public bool IsReady => FState == EnumeratorState.Ready;
        /// <summary>
        /// Get or set a value indicating whether this enumerator has exhausted it's data source.
        /// </summary>
        public bool IsExhausted {
            get => FState == EnumeratorState.Exhausted;
            set
            {
                if (FState == EnumeratorState.Exhausted && !value)
                    FState = EnumeratorState.Ready;
                else if (FState == EnumeratorState.Ready && value)
                    FState = EnumeratorState.Exhausted;
            }
        }
        /// <summary>
        /// Get a valud indicating whether this enumerator has been disposed.
        /// </summary>
        public bool IsDisposed => FState == EnumeratorState.Disposed;
    }
}

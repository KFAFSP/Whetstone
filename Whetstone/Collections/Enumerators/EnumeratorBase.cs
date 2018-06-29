using System;

using JetBrains.Annotations;

using Whetstone.Contracts;

using SC = System.Collections;
using SCG = System.Collections.Generic;

namespace Whetstone.Collections.Enumerators
{
    /// <summary>
    /// Base class for implementing an <see cref="SCG.IEnumerator{T}"/>.
    /// </summary>
    /// <typeparam name="T">The item type.</typeparam>
    [PublicAPI]
    public abstract class EnumeratorBase<T> : Disposable,
        SCG.IEnumerator<T>
    {
        EnumeratorState FState;
        T FCurrent;

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
        /// <returns>
        /// <see langword="true"/> if successful; <see langword="false"/> if exhausted.
        /// </returns>
        protected abstract bool DoMoveNext(out T AValue);
        /// <summary>
        /// Reset a bound enumerator.
        /// </summary>
        protected abstract void DoReset();

        /// <summary>
        /// Throw an <see cref="InvalidOperationException"/> if this <see cref="EnumeratorBase{T}"/>
        /// is unbound.
        /// </summary>
        /// <exception cref="InvalidOperationException">Enumerator is not bound.</exception>
        protected void ThrowIfUnbound()
        {
            if (!IsBound) throw new InvalidOperationException("Enumerator is not bound.");
        }
        /// <summary>
        /// Throw an <see cref="InvalidOperationException"/> if this <see cref="EnumeratorBase{T}"/>
        /// is not ready.
        /// </summary>
        /// <exception cref="InvalidOperationException">Enumerator is not ready.</exception>
        protected void ThrowIfNotReady()
        {
            if (!IsReady) throw new InvalidOperationException("Enumerator is not ready.");
        }

        /// <summary>
        /// Ensure that the enumerator is bound.
        /// </summary>
        [BindsEnumerator]
        protected void Bind()
        {
            if (IsBound) return;
            ThrowIfDisposed();

            DoBind();
            FState = EnumeratorState.Ready;
        }

        #region IDisposable
        /// <inheritdoc />
        protected override void Dispose(bool ADisposing)
        {
            if (ADisposing && IsBound) DoUnbind();

            FState = EnumeratorState.Disposed;
            base.Dispose(ADisposing);
        }
        #endregion

        #region IEnumerator
        /// <inheritdoc />
        [BindsEnumerator]
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
            ThrowIfUnbound();

            DoReset();
        }

        object SC.IEnumerator.Current => Current;
        #endregion

        #region IEnumerator<T>
        /// <inheritdoc />
        public T Current
        {
            get
            {
                ThrowIfNotReady();
                return FCurrent;
            }
            protected set => FCurrent = value;
        }
        #endregion

        /// <summary>
        /// Get a value indicating whether this enumerator is bound to the datasource.
        /// </summary>
        public bool IsBound =>
            FState == EnumeratorState.Ready
            || FState == EnumeratorState.Exhausted;
        /// <summary>
        /// Get a value indicating whether this enumerator is in enumeration.
        /// </summary>
        public bool IsReady => FState == EnumeratorState.Ready;
        /// <summary>
        /// Get or set a value indicating whether this enumerator has exhausted it's data source.
        /// </summary>
        public bool IsExhausted {
            get => FState == EnumeratorState.Exhausted;
            protected set
            {
                if (FState == EnumeratorState.Exhausted && !value)
                    FState = EnumeratorState.Ready;
                else if (FState == EnumeratorState.Ready && value)
                    FState = EnumeratorState.Exhausted;
            }
        }
    }
}

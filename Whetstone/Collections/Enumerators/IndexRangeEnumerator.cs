using System;

using JetBrains.Annotations;

using SCG = System.Collections.Generic;

namespace Whetstone.Collections.Enumerators
{
    /// <summary>
    /// Base class for <see cref="SCG.IEnumerator{T}"/>s that enumerate over indexed data sources.
    /// </summary>
    /// <typeparam name="T">The item type.</typeparam>
    [PublicAPI]
    public abstract class IndexRangeEnumerator<T> : EnumeratorBase<T>,
        IReversable,
        IFixedLength,
        IRandomAccess<int>,
        IPartitionable
    {
        private EnumeratorDirection FDirection;
        private int FLower;
        private int FUpper;

        private int FIndex;

        /// <summary>
        /// Fetch the data at the specified index.
        /// </summary>
        /// <param name="AIndex">The index.</param>
        /// <returns>The data at <paramref name="AIndex"/>.</returns>
        protected abstract T Fetch(int AIndex);
        /// <summary>
        /// Called when the range of this enumerator has changed.
        /// </summary>
        protected virtual void OnRangeChanged()
        { }
        /// <summary>
        /// Create a new copy of this enumerator with different bounds.
        /// </summary>
        /// <param name="ALower">The lower bound.</param>
        /// <param name="AUpper">The upper bound.</param>
        /// <returns>A new <see cref="IndexRangeEnumerator{T}"/> instance.</returns>
        protected abstract IndexRangeEnumerator<T> NewPartition(int ALower, int AUpper);

        #region EnumeratorBase<T> overrides
        /// <inheritdoc />
        protected override void DoBind()
        {
            DoReset();
        }

        /// <inheritdoc />
        protected override bool DoMoveNext(out T AValue)
        {
            AValue = default(T);

            switch (FDirection)
            {
                case EnumeratorDirection.Forward:
                    if (++FIndex > FUpper)
                        return false;
                    break;

                case EnumeratorDirection.Backward:
                    if (--FIndex < FLower)
                        return false;
                    break;
            }

            AValue = Fetch(FIndex);
            return true;
        }
        /// <inheritdoc />
        protected override void DoReset()
        {
            switch (FDirection)
            {
                case EnumeratorDirection.Forward:
                    FIndex = FLower - 1;
                    break;

                case EnumeratorDirection.Backward:
                    FIndex = FUpper + 1;
                    break;

                default:
                    throw new NotImplementedException();
            }

            if (FIndex > FUpper || FIndex < FLower)
                IsExhausted = true;
        }
        #endregion

        #region IDirected
        /// <inheritdoc />
        public EnumeratorDirection Direction => FDirection;
        #endregion

        #region IReversable
        /// <inheritdoc />
        public void Reverse()
        {
            Bind();

            FDirection = FDirection == EnumeratorDirection.Forward
                ? EnumeratorDirection.Backward
                : EnumeratorDirection.Forward;

            IsExhausted = false;
        }
        #endregion

        #region IFixedLength
        /// <inheritdoc />
        public int Remaining
        {
            get
            {
                Bind();

                return Math.Max(0, FDirection == EnumeratorDirection.Forward
                    ? FUpper - FIndex
                    : FIndex - FLower);
            }
        }

        /// <inheritdoc />
        public EnumeratorLengthType LengthType => EnumeratorLengthType.Exact;
        #endregion

        #region IIndexed<int>
        SCG.IComparer<int> IIndexed<int>.IndexComparer => SCG.Comparer<int>.Default;

        /// <inheritdoc />
        public int Index => IsReady ? FIndex : throw new InvalidOperationException("Enumerator is out of bounds.");
        #endregion

        #region IRandomAccess<int>
        /// <inheritdoc />
        public void MoveTo(int AIndex)
        {
            Bind();

            if (AIndex < FLower || AIndex > FUpper)
                throw new ArgumentOutOfRangeException(nameof(AIndex));

            FIndex = AIndex;
            IsExhausted = false;
            Current = Fetch(FIndex);
        }
        #endregion

        #region IPartitionable
        /// <inheritdoc />
        public IPartitionable TryPartition()
        {
            if (IsExhausted) return null;

            Bind();

            int remain, split, temp;
            IndexRangeEnumerator<T> part;
            switch (FDirection)
            {
                case EnumeratorDirection.Forward:
                    remain = FUpper - FIndex;
                    if (remain < 2) return null;
                    split = FIndex + remain / 2;

                    temp = FUpper;
                    FUpper = split;

                    part = NewPartition(FUpper + 1, temp);
                    part.Bind();
                    OnRangeChanged();
                    return part;

                case EnumeratorDirection.Backward:
                    remain = FIndex - FLower;
                    if (remain < 2) return null;
                    split = FIndex - remain / 2;

                    temp = FLower;
                    FLower = split;

                    part = NewPartition(temp, FLower - 1);
                    part.Bind();
                    OnRangeChanged();
                    return part;

                default:
                    throw new NotImplementedException();
            }
        }
        #endregion

        /// <summary>
        /// Get the lower bound of this enumerator.
        /// </summary>
        public int Lower => FLower;
        /// <summary>
        /// Get the upper bound of this enumerator.
        /// </summary>
        public int Upper => FUpper;
    }
}

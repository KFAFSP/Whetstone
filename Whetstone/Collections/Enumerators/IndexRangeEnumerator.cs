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
        /// <returns>A new <see cref="IndexRangeEnumerator{T}"/> instance, or <see langword="null"/> if not possible.</returns>
        [CanBeNull]
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

            switch (Direction)
            {
                case EnumeratorDirection.Forward:
                    if (++FIndex > Upper)
                        return false;
                    break;

                case EnumeratorDirection.Backward:
                    if (--FIndex < Lower)
                        return false;
                    break;
            }

            AValue = Fetch(FIndex);
            return true;
        }
        /// <inheritdoc />
        protected override void DoReset()
        {
            switch (Direction)
            {
                case EnumeratorDirection.Forward:
                    FIndex = Lower - 1;
                    break;

                case EnumeratorDirection.Backward:
                    FIndex = Upper + 1;
                    break;

                default:
                    throw new NotImplementedException();
            }

            if (FIndex > Upper || FIndex < Lower)
                IsExhausted = true;
        }
        #endregion

        #region IDirected
        /// <inheritdoc />
        public EnumeratorDirection Direction { get; private set; }
        #endregion

        #region IReversable
        /// <inheritdoc />
        public void Reverse()
        {
            Bind();

            Direction = Direction == EnumeratorDirection.Forward
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

                return Math.Max(0, Direction == EnumeratorDirection.Forward
                    ? Upper - FIndex
                    : FIndex - Lower);
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

            if (AIndex < Lower || AIndex > Upper)
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

            int remain, split;
            IndexRangeEnumerator<T> part;
            switch (Direction)
            {
                case EnumeratorDirection.Forward:
                    remain = Upper - FIndex;
                    if (remain < 2) return null;
                    split = FIndex + remain / 2;

                    part = NewPartition(split + 1, Upper);
                    if (part == null) return null;
                    Upper = split;

                    part.Bind();
                    OnRangeChanged();
                    return part;

                case EnumeratorDirection.Backward:
                    remain = FIndex - Lower;
                    if (remain < 2) return null;
                    split = FIndex - remain / 2;

                    part = NewPartition(Lower, split - 1);
                    if (part == null) return null;
                    Lower = split;
                    
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
        public int Lower { get; private set; }
        /// <summary>
        /// Get the upper bound of this enumerator.
        /// </summary>
        public int Upper { get; private set; }
    }
}

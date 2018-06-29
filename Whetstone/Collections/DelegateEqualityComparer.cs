using System;

using SC = System.Collections;
using SCG = System.Collections.Generic;

using JetBrains.Annotations;

namespace Whetstone.Collections
{
    /// <summary>
    /// An <see cref="SCG.IEqualityComparer{T}"/> that uses delegates.
    /// </summary>
    /// <typeparam name="T">The compared type.</typeparam>
    [PublicAPI]
    public sealed class DelegateEqualityComparer<T> :
        SCG.IEqualityComparer<T>,
        SC.IEqualityComparer
    {
        static readonly Func<T, int> _FNullHasher = X => 0;

        [NotNull] readonly Func<T, T, bool> FEquals;
        [NotNull] readonly Func<T, int> FHash;

        /// <summary>
        /// Create a new <see cref="DelegateEqualityComparer{T}"/> instance.
        /// </summary>
        /// <param name="AEquals">The equals delegate.</param>
        /// <param name="AHash">The hashing delegate.</param>
        public DelegateEqualityComparer(
            [NotNull] Func<T, T, bool> AEquals,
            [CanBeNull] Func<T, int> AHash = null
        )
        {
            FEquals = AEquals ?? throw new ArgumentNullException(nameof(AEquals));
            FHash = AHash ?? _FNullHasher;
        }

        #region IEqualityComparer<T>
        /// <inheritdoc />
        public bool Equals(T AX, T AY)
        {
            return FEquals(AX, AY);
        }

        /// <inheritdoc />
        public int GetHashCode(T AObject)
        {
            return FHash(AObject);
        }
        #endregion

        #region IEqualityComparer
        bool SC.IEqualityComparer.Equals(object AX, object AY)
        {
            if ((AX != null && !(AX is T)) || (AY != null && !(AY is T)))
                throw new ArgumentException("Incomparable types.");

            return Equals((T)AX, (T)AY);
        }

        int SC.IEqualityComparer.GetHashCode([NotNull] object AObject)
        {
            if (AObject != null && !(AObject is T))
                throw new ArgumentException("Unhashable type.");

            return GetHashCode((T)AObject);
        }
        #endregion
    }
}
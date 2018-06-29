using System;

using SC = System.Collections;
using SCG = System.Collections.Generic;

using JetBrains.Annotations;

namespace Whetstone.Collections
{
    /// <summary>
    /// An <see cref="SCG.IComparer{T}"/> that uses delegates.
    /// </summary>
    /// <typeparam name="T">The compared type.</typeparam>
    [PublicAPI]
    public sealed class DelegateComparer<T> :
        SCG.IComparer<T>,
        SC.IComparer
    {
        [NotNull] readonly Func<T, T, int> FCompare;

        /// <summary>
        /// Create a new <see cref="DelegateComparer{T}"/> instance.
        /// </summary>
        /// <param name="ACompare">The comparison delegate.</param>
        public DelegateComparer([NotNull] Func<T, T, int> ACompare)
        {
            FCompare = ACompare ?? throw new ArgumentNullException(nameof(ACompare));
        }

        #region IComparer<T>
        /// <inheritdoc />
        public int Compare(T AX, T AY)
        {
            return FCompare(AX, AY);
        }
        #endregion

        #region IComparer
        int SC.IComparer.Compare(object AX, object AY)
        {
            if ((AX != null && !(AX is T)) || (AY != null && !(AY is T)))
                throw new ArgumentException("Incomparable types.");

            return Compare((T)AX, (T)AY);
        }
        #endregion
    }
}
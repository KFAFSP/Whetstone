using System;

using JetBrains.Annotations;

using SC = System.Collections;
using SCG = System.Collections.Generic;

namespace Whetstone.Collections.Enumerators
{
    /// <summary>
    /// Trait interface for a <see cref="SC.IEnumerator"/> that enumerates over an indexed sequence.
    /// </summary>
    /// <remarks>
    /// Indices are not allowed to be <see langword="null"/>.
    /// </remarks>
    /// <typeparam name="TIndex">The type of the index.</typeparam>
    [PublicAPI]
    public interface IIndexed<TIndex> :
        SC.IEnumerator,
        IDisposable
    {
        /// <summary>
        /// Get the comparer for the item indices.
        /// </summary>
        [NotNull]
        SCG.IComparer<TIndex> IndexComparer { [Pure] get; }

        /// <summary>
        /// Get the index of the current item.
        /// </summary>
        /// <remarks>
        /// Getting this value before the start or after the end of the enumeration results in an
        /// <see cref="InvalidOperationException"/>.
        /// </remarks>
        /// <exception cref="InvalidOperationException">The enumerator is out of bounds.</exception>
        [NotNull]
        TIndex Index { [Pure] get; }
    }
}
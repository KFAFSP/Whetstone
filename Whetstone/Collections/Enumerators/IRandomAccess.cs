using System;

using JetBrains.Annotations;

namespace Whetstone.Collections.Enumerators
{
    /// <summary>
    /// Trait interface for a <see cref="IIndexed{TIndex}"/> enumerator that can jump to a specified index.
    /// </summary>
    /// <typeparam name="TIndex">The type of the index.</typeparam>
    [PublicAPI]
    public interface IRandomAccess<TIndex> : IIndexed<TIndex>
    {
        /// <summary>
        /// Get or set the index the <see cref="IRandomAccess{TIndex}"/> enumerator is located at.
        /// </summary>
        /// <remarks>
        /// Setting this property will cause the enumerator to bind.
        /// </remarks>
        /// <param name="value">The index to jump to, may not be <see langword="null"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/> is outside the enumerator's bounds.</exception>
        /// <exception cref="InvalidOperationException">The enumerator is disposed.</exception>
        [NotNull]
        new TIndex Index { [Pure] get; set; }
    }
}
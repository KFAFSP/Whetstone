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
        /// Move the <see cref="IRandomAccess{TIndex}"/> enumerator to the specified index.
        /// </summary>
        /// <remarks>
        /// Calls to this method are not valid before the start or after the end.
        /// </remarks>
        /// <param name="AIndex">The index to jump to, may not be <see langword="null"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="AIndex"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="AIndex"/> is outside the enumerator's bounds.</exception>
        /// <exception cref="InvalidOperationException">The enumerator is out of bounds.</exception>
        void MoveTo([NotNull] TIndex AIndex);
    }
}
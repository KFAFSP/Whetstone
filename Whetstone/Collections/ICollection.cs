using JetBrains.Annotations;

using SC = System.Collections;
using SCG = System.Collections.Generic;

namespace Whetstone.Collections
{
    /// <summary>
    /// Represents a finite collection of items.
    /// </summary>
    [PublicAPI]
    public interface ICollection : SC.IEnumerable
    {
        /// <summary>
        /// Check whether this collection contains the specified item.
        /// </summary>
        /// <param name="AItem">The item to search for.</param>
        /// <returns><c>true</c> if <paramref name="AItem"/> is part of the collection; otherwise <c>false</c>.</returns>
        /// <remarks>
        /// Equality is determined using the <see cref="EqualsEqualityComparer"/>.
        /// </remarks>
        [Pure]
        bool Contains(object AItem);

        /// <summary>
        /// Get the number of items in the collection.
        /// </summary>
        int Size { [Pure] get; }
    }

    /// <summary>
    /// Represents a finite collection of strongly typed items.
    /// </summary>
    /// <typeparam name="T">The item type.</typeparam>
    [PublicAPI]
    public interface ICollection<T> : ICollection, SCG.IEnumerable<T>
    {
        /// <summary>
        /// Check whether this collection contains the specified item.
        /// </summary>
        /// <param name="AItem">The item to search for.</param>
        /// <returns><c>true</c> if <paramref name="AItem"/> is part of the collection; otherwise <c>false</c>.</returns>
        /// <remarks>
        /// Equality is determined using the <see cref="EqualsEqualityComparer"/>.
        /// </remarks>
        [Pure]
        bool Contains(T AItem);
    }
}

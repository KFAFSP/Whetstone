using SC = System.Collections;
using SCG = System.Collections.Generic;

using JetBrains.Annotations;

namespace Whetstone.Collections
{
    /// <summary>
    /// Represents a <see cref="ICollection"/> of unique items.
    /// </summary>
    [PublicAPI]
    public interface ISet : ICollection
    {
        /// <summary>
        /// Check whether the specified item is contained in the set.
        /// </summary>
        /// <param name="AItem">The item to look for.</param>
        /// <returns><c>true</c> if <paramref name="AItem"/> is part of the set; otherwise <c>false</c>.</returns>
        /// <remarks>
        /// Equality is checked using <see cref="EqualityComparer"/>.
        /// </remarks>
        new bool Contains(object AItem);

        /// <summary>
        /// Get the <see cref="SC.IEqualityComparer"/> for items of this set.
        /// </summary>
        [NotNull]
        SC.IEqualityComparer EqualityComparer { [Pure] get; }
    }

    /// <summary>
    /// Represents a strongly typed <see cref="ICollection{T}"/> of unique items.
    /// </summary>
    /// <typeparam name="T">The item type.</typeparam>
    [PublicAPI]
    public interface ISet<T> : ISet, ICollection<T>
    {
        /// <summary>
        /// Check whether the specified item is contained in the set.
        /// </summary>
        /// <param name="AItem">The item to look for.</param>
        /// <returns><c>true</c> if <paramref name="AItem"/> is part of the set; otherwise <c>false</c>.</returns>
        /// <remarks>
        /// Equality is checked using <see cref="EqualityComparer"/>.
        /// </remarks>
        new bool Contains(T AItem);

        /// <summary>
        /// Get the <see cref="SCG.IEqualityComparer{T}"/> for the items of this set.
        /// </summary>
        [NotNull]
        new SCG.IEqualityComparer<T> EqualityComparer { [Pure] get; }
    }
}

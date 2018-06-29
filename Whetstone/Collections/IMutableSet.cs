using System;

using JetBrains.Annotations;

namespace Whetstone.Collections
{
    /// <summary>
    /// Represents a mutable <see cref="ISet"/>.
    /// </summary>
    [PublicAPI]
    public interface IMutableSet :
        ISet,
        IMutableCollection
    {
        /// <summary>
        /// Try to add an item to the set.
        /// </summary>
        /// <param name="AItem">The item to add.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="AItem"/> was added to the set;
        /// <see langword="false"/> if it already exists.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="AItem"/> is of an invalid type.
        /// </exception>
        new bool Add(object AItem);

        /// <summary>
        /// Try to remove an item from the set.
        /// </summary>
        /// <param name="AItem">The item to remove.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="AItem"/> was removed from the set;
        /// <see langword="false"/> if it isn't part of the set.
        /// </returns>
        new bool Remove(object AItem);
    }

    /// <summary>
    /// Represents a mutable strongly typed <see cref="ISet{T}"/>.
    /// </summary>
    /// <typeparam name="T">The item type.</typeparam>
    [PublicAPI]
    public interface IMutableSet<T> :
        ISet<T>,
        IMutableSet,
        IMutableCollection<T>
    {
        /// <summary>
        /// Try to add an item to the set.
        /// </summary>
        /// <param name="AItem">The item to add.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="AItem"/> was added to the set;
        /// <see langword="false"/> if it already exists.
        /// </returns>
        new bool Add(T AItem);

        /// <summary>
        /// Try to remove an item from the set.
        /// </summary>
        /// <param name="AItem">The item to remove.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="AItem"/> was removed from the set;
        /// <see langword="false"/> if it isn't part of the set.
        /// </returns>
        new bool Remove(T AItem);
    }
}

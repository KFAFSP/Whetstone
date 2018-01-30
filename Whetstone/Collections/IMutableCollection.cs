using JetBrains.Annotations;

namespace Whetstone.Collections
{
    /// <summary>
    /// Represents a mutable <see cref="ICollection"/> of items.
    /// </summary>
    [PublicAPI]
    public interface IMutableCollection : ICollection
    {
        /// <summary>
        /// Add an item to the collection.
        /// </summary>
        /// <param name="AItem">The item to add.</param>
        void Add(object AItem);
        /// <summary>
        /// Remove an item from the collection.
        /// </summary>
        /// <param name="AItem">The item to remove.</param>
        /// <returns><c>true</c> if the item was removed; <c>false</c> if it was not found.</returns>
        /// <remarks>
        /// Equality is determined using the <see cref="EqualsEqualityComparer"/>.
        /// If there are multiple occurrences of <paramref name="AItem"/>, only one is removed.
        /// </remarks>
        bool Remove(object AItem);

        /// <summary>
        /// Remove all items from the collection.
        /// </summary>
        void Clear();
    }

    /// <summary>
    /// Represents a mutable <see cref="ICollection{T}"/> of strongly typed items.
    /// </summary>
    /// <typeparam name="T">The item type.</typeparam>
    [PublicAPI]
    public interface IMutableCollection<T> : IMutableCollection, ICollection<T>
    {
        /// <summary>
        /// Add an item to the collection.
        /// </summary>
        /// <param name="AItem">The item to add.</param>
        void Add(T AItem);
        /// <summary>
        /// Remove an item from the collection.
        /// </summary>
        /// <param name="AItem">The item to remove.</param>
        /// <returns><c>true</c> if the item was removed; <c>false</c> if it was not found.</returns>
        /// <remarks>
        /// Equality is determined using the <see cref="EqualsEqualityComparer"/>.
        /// If there are multiple occurrences of <paramref name="AItem"/>, only one is removed.
        /// </remarks>
        bool Remove(T AItem);
    }
}

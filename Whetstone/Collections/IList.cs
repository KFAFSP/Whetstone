using System;

using JetBrains.Annotations;

using Whetstone.Contracts;

namespace Whetstone.Collections
{
    /// <summary>
    /// Represents a <see cref="ICollection"/> with contiguous indices starting at 0.
    /// </summary>
    [PublicAPI]
    public interface IList : ICollection
    {
        /// <summary>
        /// Get the index of the specified item.
        /// </summary>
        /// <param name="AItem">The item to search for.</param>
        /// <returns>The index of <paramref name="AItem"/>, or -1 if not found.</returns>
        /// <remarks>
        /// If there are multiple occurrences of <paramref name="AItem"/>, the index of
        /// one of them is returned.
        /// Equality is determined using the <see cref="EqualsEqualityComparer"/>.
        /// </remarks>
        [Pure]
        int IndexOf(object AItem);

        /// <summary>
        /// Try to get the item at the specified index.
        /// </summary>
        /// <param name="AIndex">The index to get.</param>
        /// <returns>
        /// An absent <see cref="Optional{T}"/> if <paramref name="AIndex"/> is out of range,
        /// or a present <see cref="Optional{T}"/> with the item at <paramref name="AIndex"/>.
        /// </returns>
        [Pure]
        Optional<object> At(int AIndex);

        /// <summary>
        /// Get the item at the specified index.
        /// </summary>
        /// <param name="AIndex">The index to get.</param>
        /// <returns>The item at <paramref name="AIndex"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="AIndex"/> is out of range.</exception>
        object this[int AIndex] { [Pure] get; }
    }

    /// <summary>
    /// Represents a strongly typed <see cref="ICollection{T}"/> with contiguous indices starting at 0.
    /// </summary>
    /// <typeparam name="T">The item type.</typeparam>
    [PublicAPI]
    public interface IList<T> : IList, ICollection<T>
    {
        /// <summary>
        /// Get the index of the specified item.
        /// </summary>
        /// <param name="AItem">The item to search for.</param>
        /// <returns>The index of <paramref name="AItem"/>, or -1 if not found.</returns>
        /// <remarks>
        /// If there are multiple occurrences of <paramref name="AItem"/>, the index of
        /// one of them is returned.
        /// Equality is determined using the <see cref="EqualsEqualityComparer"/>.
        /// </remarks>
        [Pure]
        int IndexOf(T AItem);

        /// <summary>
        /// Try to get the item at the specified index.
        /// </summary>
        /// <param name="AIndex">The index to get.</param>
        /// <returns>
        /// An absent <see cref="Optional{T}"/> if <paramref name="AIndex"/> is out of range,
        /// or a present <see cref="Optional{T}"/> with the item at <paramref name="AIndex"/>.
        /// </returns>
        [Pure]
        new Optional<T> At(int AIndex);

        /// <summary>
        /// Get the item at the specified index.
        /// </summary>
        /// <param name="AIndex">The index to get.</param>
        /// <returns>The item at <paramref name="AIndex"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="AIndex"/> is out of range.</exception>
        new T this[int AIndex] { [Pure] get; }
    }
}

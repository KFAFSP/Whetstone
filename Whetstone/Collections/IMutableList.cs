using System;

using JetBrains.Annotations;

using Whetstone.Contracts;

namespace Whetstone.Collections
{
    /// <summary>
    /// Represents a mutable <see cref="IList"/>.
    /// </summary>
    [PublicAPI]
    public interface IMutableList : IList, IMutableCollection
    {
        /// <summary>
        /// Add an item to the list.
        /// </summary>
        /// <param name="AItem">The item to add.</param>
        /// <returns>The index at which <paramref name="AItem"/> was added.</returns>
        /// <remarks>
        /// The implementation is not required to append to the end of the list.
        /// If an explicit append is desired, <see cref="InsertAt"/> should be used.
        /// This operation should be the fasted possible add for the given list implementation.
        /// </remarks>
        /// <exception cref="ArgumentException"><paramref name="AItem"/> is of an invalid type.</exception>
        new int Add(object AItem);

        /// <summary>
        /// Insert an item at the specified index, shifting logically forward.
        /// </summary>
        /// <param name="AIndex">The index at which to insert.</param>
        /// <param name="AItem">The item to insert.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="AIndex"/> is out of range.</exception>
        /// <remarks>
        /// <paramref name="AIndex"/> can be <see cref="ICollection.Size"/> to insert at the end.
        /// </remarks>
        /// <exception cref="ArgumentException"><paramref name="AItem"/> is of an invalid type.</exception>
        void InsertAt(int AIndex, object AItem);

        /// <summary>
        /// Try to remove the item at the specified index.
        /// </summary>
        /// <param name="AIndex">The index at which to remove.</param>
        /// <returns>
        /// An absent <see cref="Optional{T}"/> if <paramref name="AIndex"/> is out of range,
        /// or a present <see cref="Optional{T}"/> with the removed item.
        /// </returns>
        Optional<object> RemoveAt(int AIndex);

        /// <summary>
        /// Get or set the item at the specified index.
        /// </summary>
        /// <param name="AIndex">The index to get.</param>
        /// <returns>The item at <paramref name="AIndex"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="AIndex"/> is out of range.</exception>
        /// <exception cref="ArgumentException"><paramref name="value"/> is of an invalid type.</exception>
        new object this[int AIndex] { [Pure] get; set; }
    }

    /// <summary>
    /// Represents a mutable strongly typed <see cref="IList{T}"/>.
    /// </summary>
    /// <typeparam name="T">The item type.</typeparam>
    [PublicAPI]
    public interface IMutableList<T> : IMutableList, IList<T>, IMutableCollection<T>
    {
        /// <summary>
        /// Add an item to the list.
        /// </summary>
        /// <param name="AItem">The item to add.</param>
        /// <returns>The index at which <paramref name="AItem"/> was added.</returns>
        /// <remarks>
        /// The implementation is not required to append to the end of the list.
        /// If an explicit append is desired, <see cref="InsertAt"/> should be used.
        /// This operation should be the fasted possible add for the given list implementation.
        /// </remarks>
        new int Add(T AItem);

        /// <summary>
        /// Insert an item at the specified index, shifting logically forward.
        /// </summary>
        /// <param name="AIndex">The index at which to insert.</param>
        /// <param name="AItem">The item to insert.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="AIndex"/> is out of range.</exception>
        /// <remarks>
        /// <paramref name="AIndex"/> can be <see cref="ICollection.Size"/> to insert at the end.
        /// </remarks>
        void InsertAt(int AIndex, T AItem);

        /// <summary>
        /// Try to remove the item at the specified index.
        /// </summary>
        /// <param name="AIndex">The index at which to remove.</param>
        /// <returns>
        /// An absent <see cref="Optional{T}"/> if <paramref name="AIndex"/> is out of range,
        /// or a present <see cref="Optional{T}"/> with the removed item.
        /// </returns>
        new Optional<T> RemoveAt(int AIndex);

        /// <summary>
        /// Get or set the item at the specified index.
        /// </summary>
        /// <param name="AIndex">The index to get.</param>
        /// <returns>The item at <paramref name="AIndex"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="AIndex"/> is out of range.</exception>
        new T this[int AIndex] { [Pure] get; set; }
    }
}

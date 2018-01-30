using System;

using JetBrains.Annotations;

namespace Whetstone.Collections.Manipulators
{
    /// <summary>
    /// Trait interface for a <see cref="IManipulator"/> that can modify the structure of the underlying sequence.
    /// </summary>
    [PublicAPI]
    public interface IStructural : IManipulator
    {
        /// <summary>
        /// Insert an item at the current position and move to the next.
        /// </summary>
        /// <remarks>
        /// Inserting is not valid before the start of the enumeration, but after the end.
        /// </remarks>
        /// <param name="AItem">The item to insert.</param>
        /// <returns><c>true</c> if the item was inserted and the enumeration moved; otherwise <c>false</c>.</returns>
        /// <exception cref="InvalidOperationException">The enumeration has not started.</exception>
        bool InsertAndMoveNext(object AItem);

        /// <summary>
        /// Removes the item at the current position and automatically moves to the next item.
        /// </summary>
        /// <remarks>
        /// This operation is not valid before the start or after the end of the enumeration.
        /// </remarks>
        /// <returns><c>true</c> if there was another item in the enumeration; otherwise <c>false</c>.</returns>
        /// <exception cref="InvalidOperationException">The enumerator is out of bounds.</exception>
        bool RemoveAndMoveNext();
    }

    /// <summary>
    /// Trait interface for a <see cref="IManipulator{T}"/> that can modify the structure of the underlying sequence.
    /// </summary>
    /// <typeparam name="T">The enumerated item type.</typeparam>
    [PublicAPI]
    public interface IStructural<T> : IStructural, IManipulator<T>
    {
        /// <summary>
        /// Insert an item at the current position and move to the next.
        /// </summary>
        /// <remarks>
        /// Inserting is not valid before the start of the enumeration, but after the end.
        /// </remarks>
        /// <param name="AItem">The item to insert.</param>
        /// <returns><c>true</c> if the item was inserted and the enumeration moved; otherwise <c>false</c>.</returns>
        /// <exception cref="InvalidOperationException">The enumeration has not started.</exception>
        bool InsertAndMoveNext(T AItem);
    }
}

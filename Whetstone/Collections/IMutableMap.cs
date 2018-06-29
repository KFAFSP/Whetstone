using System;

using SCG = System.Collections.Generic;

using JetBrains.Annotations;

using Whetstone.Contracts;

namespace Whetstone.Collections
{
    /// <summary>
    /// Represents a mutable <see cref="IMap"/>.
    /// </summary>
    [PublicAPI]
    public interface IMutableMap :
        IMap,
        IMutableCollection<IKeyValuePair>
    {
        /// <summary>
        /// Set the value associated with the specified key.
        /// </summary>
        /// <param name="AKey">The key to set.</param>
        /// <param name="AValue">The value to put.</param>
        /// <returns>
        /// An absent <see cref="Optional{T}"/> if there was no previous value for
        /// <paramref name="AKey"/>, or a present <see cref="Optional{T}"/> with the overwritten
        /// value.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="AKey"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="AKey"/> is of an invalid type.
        /// </exception>
        Optional<object> Put([NotNull] object AKey, object AValue);

        /// <summary>
        /// Try to delete the value for the specified key.
        /// </summary>
        /// <param name="AKey">The key to drop.</param>
        /// <returns>
        /// An absent <see cref="Optional{T}"/> if <paramref name="AKey"/> was not found,
        /// or a present <see cref="Optional{T}"/> with the dropped value.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="AKey"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="AKey"/> is of an invalid type.
        /// </exception>
        Optional<object> Drop([NotNull] object AKey);

        /// <summary>
        /// Get the <see cref="IMutableSet"/> of keys.
        /// </summary>
        [NotNull]
        new IMutableSet Keys { get; }

        /// <summary>
        /// Get or set the value associated to the specified key.
        /// </summary>
        /// <param name="AKey">The key to lookup.</param>
        /// <returns>The value associated to <paramref name="AKey"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="AKey"/> is <see langword="null"/>.
        /// </exception>
        new object this[[NotNull] object AKey] { [Pure] get; set; }
    }

    /// <summary>
    /// Represents a strongly typed mutable <see cref="IMap{K,V}"/>.
    /// </summary>
    /// <typeparam name="K">The key type.</typeparam>
    /// <typeparam name="V">The value type.</typeparam>
    [PublicAPI]
    public interface IMutableMap<K, V> :
        IMap<K, V>,
        IMutableMap,
        IMutableCollection<IKeyValuePair<K, V>>
    {
        /// <summary>
        /// Set the value associated with the specified key.
        /// </summary>
        /// <param name="AKey">The key to set.</param>
        /// <param name="AValue">The value to put.</param>
        /// <returns>
        /// An absent <see cref="Optional{T}"/> if there was no previous value for
        /// <paramref name="AKey"/>, or a present <see cref="Optional{T}"/> with the overwritten
        /// value.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="AKey"/> is <see langword="null"/>.
        /// </exception>
        Optional<V> Put([NotNull] K AKey, V AValue);

        /// <summary>
        /// Try to delete the value for the specified key.
        /// </summary>
        /// <param name="AKey">The key to drop.</param>
        /// <returns>
        /// An absent <see cref="Optional{T}"/> if <paramref name="AKey"/> was not found,
        /// or a present <see cref="Optional{T}"/> with the dropped value.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="AKey"/> is <see langword="null"/>.
        /// </exception>
        Optional<V> Drop([NotNull] K AKey);

        /// <summary>
        /// Get the <see cref="IMutableSet{T}"/> of keys.
        /// </summary>
        [NotNull]
        new IMutableSet<K> Keys { get; }

        // Resolves ambiguity of this interface.
        /// <inheritdoc cref="SCG.IEnumerable{T}.GetEnumerator()"/>
        new SCG.IEnumerator<KeyValuePair<K, V>> GetEnumerator();

        /// <summary>
        /// Get or set the value associated to the specified key.
        /// </summary>
        /// <param name="AKey">The key to lookup.</param>
        /// <returns>The value associated to <paramref name="AKey"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="AKey"/> is <see langword="null"/>.
        /// </exception>
        new V this[[NotNull] K AKey] { [Pure] get; set; }
    }
}

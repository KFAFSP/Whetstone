using System;

using SCG = System.Collections.Generic;

using JetBrains.Annotations;

using Whetstone.Contracts;

namespace Whetstone.Collections
{
    /// <summary>
    /// Represents a mutable <see cref="ILookup"/>.
    /// </summary>
    [PublicAPI]
    public interface IMutableLookup : ILookup, IMutableCollection<IKeyValuePair>
    {
        /// <summary>
        /// Try to delete the value for the specified key.
        /// </summary>
        /// <param name="AKey">The key to drop.</param>
        /// <returns>
        /// An absent <see cref="Optional{T}"/> if <paramref name="AKey"/> was not found,
        /// or a present <see cref="Optional{T}"/> with the dropped value.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="AKey"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="AKey"/> is of an invalid type.</exception>
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
        /// <exception cref="ArgumentNullException"><paramref name="AKey"/> is <see langword="null"/>.</exception>
        new object this[[NotNull] object AKey] { [Pure] get; set; }
    }

    /// <summary>
    /// Represents a strongly typed mutable <see cref="ILookup{K,V}"/>.
    /// </summary>
    /// <typeparam name="K">The key type.</typeparam>
    /// <typeparam name="V">The value type.</typeparam>
    [PublicAPI]
    public interface IMutableLookup<K, V> : ILookup<K, V>, IMutableLookup, IMutableCollection<IKeyValuePair<K, V>>
    {
        /// <summary>
        /// Try to delete the value for the specified key.
        /// </summary>
        /// <param name="AKey">The key to drop.</param>
        /// <returns>
        /// An absent <see cref="Optional{T}"/> if <paramref name="AKey"/> was not found,
        /// or a present <see cref="Optional{T}"/> with the dropped value.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="AKey"/> is <see langword="null"/>.</exception>
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
        /// <exception cref="ArgumentNullException"><paramref name="AKey"/> is <see langword="null"/>.</exception>
        new V this[[NotNull] K AKey] { [Pure] get; set; }
    }
}

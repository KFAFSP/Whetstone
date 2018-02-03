using System;

using SCG = System.Collections.Generic;

using JetBrains.Annotations;

using Whetstone.Contracts;

namespace Whetstone.Collections
{
    /// <summary>
    /// Represents a <see cref="ICollection"/> of values associated to unique keys.
    /// </summary>
    [PublicAPI]
    public interface ILookup : ICollection<IKeyValuePair>
    {
        /// <summary>
        /// Try to get the value for the specified key.
        /// </summary>
        /// <param name="AKey">The key to lookup.</param>
        /// <returns>
        /// An absent <see cref="Optional{T}"/> if <paramref name="AKey"/> has no associated value,
        /// or a present <see cref="Optional{T}"/> with the value for <paramref name="AKey"/>.
        /// </returns>
        /// <remarks>
        /// The key equality is defined by the <see cref="Keys"/> set equality comparer.
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="AKey"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="AKey"/> is of an invalid type.</exception>
        [Pure]
        Optional<object> Get([NotNull] object AKey);

        /// <summary>
        /// Get the <see cref="ISet"/> of keys.
        /// </summary>
        [NotNull]
        ISet Keys { [Pure] get; }
        /// <summary>
        /// Get the <see cref="ICollection"/> of values.
        /// </summary>
        [NotNull]
        ICollection Values { [Pure] get; }

        /// <summary>
        /// Get the value associated to the specified key.
        /// </summary>
        /// <param name="AKey">The key to lookup.</param>
        /// <returns>The value associated to <paramref name="AKey"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="AKey"/> is <see langword="null"/>.</exception>
        /// <exception cref="KeyNotFoundException"><paramref name="AKey"/> is not part of the lookup.</exception>
        object this[[NotNull] object AKey] { [Pure] get; }
    }

    /// <summary>
    /// Represents a strongly typed <see cref="ICollection{T}"/> if values associated to unique keys.
    /// </summary>
    /// <typeparam name="K">The key type.</typeparam>
    /// <typeparam name="V">The value type.</typeparam>
    [PublicAPI]
    public interface ILookup<K, V> : ILookup, ICollection<IKeyValuePair<K, V>>
    {
        /// <summary>
        /// Try to get the value for the specified key.
        /// </summary>
        /// <param name="AKey">The key to lookup.</param>
        /// <returns>
        /// An absent <see cref="Optional{T}"/> if <paramref name="AKey"/> has no associated value,
        /// or a present <see cref="Optional{T}"/> with the value for <paramref name="AKey"/>.
        /// </returns>
        /// <remarks>
        /// The key equality is defined by the <see cref="Keys"/> set equality comparer.
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="AKey"/> is <see langword="null"/>.</exception>
        [Pure]
        Optional<V> Get([NotNull] K AKey);

        /// <summary>
        /// Get the <see cref="ISet{T}"/> of keys.
        /// </summary>
        [NotNull]
        new ISet<K> Keys { [Pure] get; }
        /// <summary>
        /// Get the <see cref="ICollection{T}"/> of values.
        /// </summary>
        [NotNull]
        new ICollection<V> Values { [Pure] get; }

        // Resolves ambiguity of this interface.
        /// <inheritdoc cref="SCG.IEnumerable{T}.GetEnumerator()"/>
        new SCG.IEnumerator<KeyValuePair<K, V>> GetEnumerator();

        /// <summary>
        /// Get the value associated to the specified key.
        /// </summary>
        /// <param name="AKey">The key to lookup.</param>
        /// <returns>The value associated to <paramref name="AKey"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="AKey"/> is <see langword="null"/>.</exception>
        /// <exception cref="KeyNotFoundException"><paramref name="AKey"/> is not part of the lookup.</exception>
        V this[[NotNull] K AKey] { [Pure] get; }
    }
}

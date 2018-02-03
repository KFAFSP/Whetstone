using JetBrains.Annotations;

namespace Whetstone.Collections
{
    /// <summary>
    /// Represents a key-value-pair.
    /// </summary>
    [PublicAPI]
    public interface IKeyValuePair
    {
        /// <summary>
        /// Get the key.
        /// </summary>
        [NotNull]
        object Key { [Pure] get; }
        /// <summary>
        /// Get the value.
        /// </summary>
        object Value { [Pure] get; }
    }

    /// <summary>
    /// Represents a strongly typed <see cref="IKeyValuePair"/>.
    /// </summary>
    /// <typeparam name="K">The key type.</typeparam>
    /// <typeparam name="V">The value type.</typeparam>
    [PublicAPI]
    public interface IKeyValuePair<out K, out V> : IKeyValuePair
    {
        /// <summary>
        /// Get the key.
        /// </summary>
        [NotNull]
        new K Key { [Pure] get; }
        /// <summary>
        /// Get the value.
        /// </summary>
        new V Value { [Pure] get; }
    }
}

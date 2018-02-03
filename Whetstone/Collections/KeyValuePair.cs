using System;

using JetBrains.Annotations;

namespace Whetstone.Collections
{
    /// <summary>
    /// Simple <see cref="IKeyValuePair{K,V}"/> implementation.
    /// </summary>
    /// <typeparam name="K">The key type.</typeparam>
    /// <typeparam name="V">The value type.</typeparam>
    [PublicAPI]
    public struct KeyValuePair<K, V> : IKeyValuePair<K, V>
    {
        private readonly K FKey;
        private readonly V FValue;

        /// <summary>
        /// Initialize a new <see cref="KeyValuePair{K,V}"/> struct.
        /// </summary>
        /// <param name="AKey">The key.</param>
        /// <param name="AValue">The value.</param>
        /// <exception cref="ArgumentNullException"><paramref name="AKey"/> is <see langword="null"/>.</exception>
        public KeyValuePair([NotNull] K AKey, V AValue)
        {
            if (AKey == null) throw new ArgumentNullException(nameof(AKey));

            FKey = AKey;
            FValue = AValue;
        }

        #region IKeyValuePair
        object IKeyValuePair.Key => Key;
        object IKeyValuePair.Value => Value;
        #endregion

        #region IKeyValuePair<K, V>
        /// <inheritdoc />
        public K Key
        {
            get
            {
                if (FKey == null)
                    throw new InvalidOperationException("Uninitialized key-value-pair.");

                return FKey;
            }
        }
        /// <inheritdoc />
        public V Value => FValue;
        #endregion
    }
}
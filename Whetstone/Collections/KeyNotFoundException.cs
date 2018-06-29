using System;

using JetBrains.Annotations;

namespace Whetstone.Collections
{
    /// <summary>
    /// Indicates a missing key in a <see cref="IMap"/>.
    /// </summary>
    [PublicAPI]
    public class KeyNotFoundException : Exception
    {
        /// <summary>
        /// Initialize a new <see cref="KeyNotFoundException"/> instance.
        /// </summary>
        /// <param name="AKey">The missing key.</param>
        public KeyNotFoundException([NotNull] object AKey)
            : base("Key not found.")
        {
            Key = AKey;
        }

        /// <summary>
        /// Get the key.
        /// </summary>
        public object Key { get; }
    }
}

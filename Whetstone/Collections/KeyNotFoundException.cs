using System;

using JetBrains.Annotations;

namespace Whetstone.Collections
{
    /// <summary>
    /// Indicates a missing key in a <see cref="ILookup"/>.
    /// </summary>
    [PublicAPI]
    public class KeyNotFoundException : Exception
    {
        private readonly object FKey;

        /// <summary>
        /// Initialize a new <see cref="KeyNotFoundException"/> instance.
        /// </summary>
        /// <param name="AKey">The missing key.</param>
        public KeyNotFoundException(object AKey)
            : this(AKey, "Key not found.")
        { }
        /// <summary>
        /// Initialize a new <see cref="KeyNotFoundException"/> instance.
        /// </summary>
        /// <param name="AKey">The missing key.</param>
        /// <param name="AMessage">The error message.</param>
        public KeyNotFoundException(object AKey, string AMessage)
            : base(AMessage)
        {
            FKey = AKey;
        }

        /// <summary>
        /// Get the key.
        /// </summary>
        public object Key => FKey;
    }
}

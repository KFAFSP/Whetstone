using JetBrains.Annotations;

using SC = System.Collections;

namespace Whetstone.Collections.Enumerators
{
    /// <summary>
    /// Represents the direction an <see cref="SC.IEnumerator"/> is advancing in.
    /// </summary>
    [PublicAPI]
    public enum EnumeratorDirection
    {
        /// <summary>
        /// Logically forward.
        /// </summary>
        Forward,
        /// <summary>
        /// Logically backward.
        /// </summary>
        Backward
    }
}
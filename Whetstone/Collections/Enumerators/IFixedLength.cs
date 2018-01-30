using JetBrains.Annotations;

using SC = System.Collections;

namespace Whetstone.Collections.Enumerators
{
    /// <summary>
    /// Trait interface for a <see cref="SC.IEnumerator"/> that enumerates a fixed amount of items.
    /// </summary>
    [PublicAPI]
    public interface IFixedLength : SC.IEnumerator
    {
        /// <summary>
        /// Get the number of items remaining in this enumeration.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The value of this property needs to follow the precision requirements specified by <see cref="LengthType"/>.
        /// Fetching this value will cause the enumerator to bind.
        /// </para>
        /// <para>
        /// This property is required the be 0 at or after the end of the enumeration.
        /// </para>
        /// </remarks>
        /// <exception cref="InvalidOperationException">The enumerator is disposed.</exception>
        int Remaining { [Pure] get; }

        /// <summary>
        /// Get the precision guarantee for <see cref="Remaining"/>.
        /// </summary>
        EnumeratorLengthType LengthType { [Pure] get; }
    }
}
using JetBrains.Annotations;

namespace Whetstone.Collections.Enumerators
{
    /// <summary>
    /// Represents the precision of the <see cref="IFixedLength.Remaining"/> property.
    /// </summary>
    [PublicAPI]
    public enum EnumeratorLengthType
    {
        /// <summary>
        /// The value is an estimate which only guarantees to be an upper bound.
        /// </summary>
        Estimated,
        /// <summary>
        /// The value is the exact amount of items left.
        /// </summary>
        Exact
    }
}
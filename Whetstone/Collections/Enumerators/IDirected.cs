using JetBrains.Annotations;

using SC = System.Collections;

namespace Whetstone.Collections.Enumerators
{
    /// <summary>
    /// Trait interface for an <see cref="SC.IEnumerator"/> that is moving in a defined direction.
    /// </summary>
    [PublicAPI]
    public interface IDirected : SC.IEnumerator
    {
        /// <summary>
        /// The <see cref="EnumeratorDirection"/> that the enumeration is moving in.
        /// </summary>
        EnumeratorDirection Direction { [Pure] get; }
    }
}

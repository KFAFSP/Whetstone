using SC = System.Collections;

using JetBrains.Annotations;

namespace Whetstone.Collections.Enumerators
{
    /// <summary>
    /// States of an <see cref="SC.IEnumerator"/>.
    /// </summary>
    [PublicAPI]
    public enum EnumeratorState
    {
        /// <summary>
        /// The enumerator has not yet been bound.
        /// </summary>
        /// <remarks>
        /// No operations are valid in this state.
        /// </remarks>
        BeforeBinding = 0,
        /// <summary>
        /// The enumerator is bound and ready.
        /// </summary>
        /// <remarks>
        /// All operations are valid in this state.
        /// </remarks>
        Bound = 1,
        /// <summary>
        /// The enumerator is bound but has reached the end.
        /// </summary>
        /// <remarks>
        /// Specifically only <see cref="SC.IEnumerator.Reset"/> is valid in this state.
        /// </remarks>
        Exhausted = 2,
        /// <summary>
        /// The enumerator has been permanently unbound and disposed.
        /// </summary>
        /// <remarks>
        /// No operations are valid in this state.
        /// </remarks>
        Disposed = 3
    }
}

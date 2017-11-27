using System;

using JetBrains.Annotations;

namespace Whetstone.Collections.Enumerators
{
    /// <summary>
    /// Trait interface for a <see cref="IDirected"/> that can reverse it's direction.
    /// </summary>
    [PublicAPI]
    public interface IReversable : IDirected
    {
        /// <summary>
        /// Reverse the direction of the enumeration.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Reversing the direction may not change the position of the enumerator.
        /// </para>
        /// <para>
        /// Calling reverse before the start or after the end results in an <see cref="InvalidOperationException"/>.
        /// </para>
        /// </remarks>
        /// <exception cref="InvalidOperationException">The enumerator is out of bounds.</exception>
        void Reverse();
    }
}
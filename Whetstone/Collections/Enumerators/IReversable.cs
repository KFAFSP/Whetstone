using System;

using JetBrains.Annotations;

namespace Whetstone.Collections.Enumerators
{
    /// <summary>
    /// Trait interface for a <see cref="IDirected"/> that can reverse it's direction.
    /// </summary>
    [PublicAPI]
    public interface IReversable :
        IDirected
    {
        /// <summary>
        /// Reverse the direction of the enumeration.
        /// </summary>
        /// <remarks>
        /// Calling this method causes the enumerator to bind.
        /// Reversing the direction may not change the position of the enumerator.
        /// </remarks>
        /// <exception cref="InvalidOperationException">The enumerator is disposed.</exception>
        [BindsEnumerator]
        void Reverse();
    }
}
using System;

using JetBrains.Annotations;

using SC = System.Collections;

namespace Whetstone.Collections.Enumerators
{
    /// <summary>
    /// Trait interface for a <see cref="SC.IEnumerator"/> that can be partitioned for parallel enumeration.
    /// </summary>
    [PublicAPI]
    public interface IPartitionable : SC.IEnumerator, IDisposable
    {
        /// <summary>
        /// Try to partition this <see cref="IPartitionable"/> enumerator into two of approximately equal workload.
        /// </summary>
        /// <remarks>
        /// Calling this method will cause the enumerator to bind.
        /// Calling this method manipulates the state of the current enumerator AND returns a new one.
        /// </remarks>
        /// <returns>A new <see cref="IPartitionable"/> that contains the second partition; or <see langword="null"/> if partitioning failed / was not feasible.</returns>
        /// <exception cref="InvalidOperationException">The enumerator is disposed.</exception>
        [MustUseReturnValue]
        [CanBeNull]
        IPartitionable TryPartition();
    }
}
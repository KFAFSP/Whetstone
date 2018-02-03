using SC = System.Collections;
using SCG = System.Collections.Generic;

using JetBrains.Annotations;

namespace Whetstone.Collections
{
    /// <summary>
    /// Represents a mutable <see cref="SC.IEnumerable"/>.
    /// </summary>
    [PublicAPI]
    public interface IManipulatable : SC.IEnumerable
    {
        /// <summary>
        /// Get a maniuplator for this <see cref="IManipulatable"/>.
        /// </summary>
        /// <returns>A <see cref="IManipulator"/> instance.</returns>
        [MustUseReturnValue]
        [NotNull]
        IManipulator GetManipulator();
    }

    /// <summary>
    /// Represents a mutable strongly typed <see cref="SCG.IEnumerable{T}"/>.
    /// </summary>
    /// <typeparam name="T">The item type.</typeparam>
    [PublicAPI]
    public interface IManipulatable<T> : SCG.IEnumerable<T>, IManipulatable
    {
        /// <summary>
        /// Get a maniuplator for this <see cref="IManipulatable{T}"/>.
        /// </summary>
        /// <returns>A <see cref="IManipulator{T}"/> instance.</returns>
        [MustUseReturnValue]
        [NotNull]
        new IManipulator<T> GetManipulator();
    }
}

using System;

using JetBrains.Annotations;

using SC = System.Collections;
using SCG = System.Collections.Generic;

namespace Whetstone.Collections
{
    /// <summary>
    /// Represents an <see cref="SC.IEnumerator"/> that is capable of manipulating the underlying
    /// sequence.
    /// </summary>
    [PublicAPI]
    public interface IManipulator :
        SC.IEnumerator
    {
        /// <summary>
        /// Get or set the value the <see cref="IManipulator"/> is currently at.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Access of this property is invalid before the start and after the end of the enumeration.
        /// </para>
        /// </remarks>
        /// <exception cref="InvalidOperationException">The enumerator is out of bounds.</exception>
        new object Current { [Pure] get; set; }
    }

    /// <summary>
    /// Represents an <see cref="SCG.IEnumerator{T}"/> that is capable of manipulating the
    /// underlying sequence.
    /// </summary>
    /// <typeparam name="T">The enumerated item type.</typeparam>
    [PublicAPI]
    public interface IManipulator<T> :
        IManipulator,
        SCG.IEnumerator<T>
    {
        /// <summary>
        /// Get or set the value the <see cref="IManipulator{T}"/> is currently at.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Access of this property is invalid before the start and after the end of the
        /// enumeration.
        /// </para>
        /// </remarks>
        /// <exception cref="InvalidOperationException">The enumerator is out of bounds.</exception>
        new T Current { [Pure] get; set; }
    }
}

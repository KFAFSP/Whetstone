using System.Runtime.CompilerServices;

using SC = System.Collections;
using SCG = System.Collections.Generic;

using JetBrains.Annotations;

namespace Whetstone.Collections
{
    /// <summary>
    /// An <see cref="SCG.IEqualityComparer{T}"/> that uses the
    /// <see cref="object.ReferenceEquals(object, object)"/> equality comparison.
    /// </summary>
    [PublicAPI]
    public sealed class ReferenceEqualityComparer :
        SCG.IEqualityComparer<object>,
        SC.IEqualityComparer
    {
        /// <summary>
        /// Singleton instance of the <see cref="ReferenceEqualityComparer"/>.
        /// </summary>
        [NotNull]
        public static readonly ReferenceEqualityComparer Instance = new ReferenceEqualityComparer();

        #region IEqualityComparer<object>
        /// <inheritdoc cref="SCG.IEqualityComparer{T}.Equals(T, T)"/>
        public new bool Equals(object AX, object AY)
        {
            return ReferenceEquals(AX, AY);
        }

        /// <inheritdoc cref="SCG.IEqualityComparer{T}.GetHashCode(T)"/>
        public int GetHashCode(object AObject)
        {
            return RuntimeHelpers.GetHashCode(AObject);
        }
        #endregion
    }
}
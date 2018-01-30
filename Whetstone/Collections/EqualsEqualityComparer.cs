using SC = System.Collections;
using SCG = System.Collections.Generic;

using JetBrains.Annotations;

namespace Whetstone.Collections
{
    /// <summary>
    /// An <see cref="SCG.IEqualityComparer{T}"/> that uses the <see cref="object.Equals(object)"/>
    /// equality comparison. The hash is computed using the reference value.
    /// </summary>
    [PublicAPI]
    public sealed class EqualsEqualityComparer : SCG.IEqualityComparer<object>, SC.IEqualityComparer
    {
        /// <summary>
        /// Singleton instance of the <see cref="EqualsEqualityComparer"/>.
        /// </summary>
        [NotNull]
        public static readonly EqualsEqualityComparer Instance = new EqualsEqualityComparer();

        #region IEqualityComparer
        /// <inheritdoc cref="SCG.IEqualityComparer{T}.Equals(T, T)"/>
        public new bool Equals(object AX, object AY)
        {
            return ReferenceEquals(AX, AY) || (AX?.Equals(AY) ?? false);
        }

        /// <inheritdoc cref="SCG.IEqualityComparer{T}.GetHashCode(T)"/>
        public int GetHashCode(object AObject)
        {
            return AObject.GetHashCode();
        }
        #endregion
    }
}

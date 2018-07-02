using System;
using System.Diagnostics;

using JetBrains.Annotations;

namespace Whetstone.Contracts
{
    /// <summary>
    /// Static helper class that provides useful methods for verifying contracts.
    /// </summary>
    [PublicAPI]
    public static class ContractHelpers
    {
        const string C_TypeMismatch = "Argument type mismatched.";

        /// <summary>
        /// Check if an arbitrary object matches the specified type constraint.
        /// </summary>
        /// <typeparam name="T">The type constraint.</typeparam>
        /// <param name="AObject">The target object.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="AObject"/> matches <typeparamref name="T"/>;
        /// otherwise <see langword="false"/>.
        /// </returns>
        [DebuggerHidden]
        public static bool IsConstrainedBy<T>(this object AObject)
        {
            var type = typeof(T);

            if (AObject == null)
                return !type.IsValueType;

            return AObject is T;
        }

        /// <summary>
        /// Check that a specified argument matches the specified type constraint ans throw a
        /// <see cref="ArgumentException"/> if it doesn't.
        /// </summary>
        /// <typeparam name="T">The type constraint.</typeparam>
        /// <param name="AParam">The target parameter value.</param>
        /// <param name="AParamName">The parameter name.</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="AParam"/> does not match <typeparamref name="T"/>.
        /// </exception>
        [DebuggerHidden]
        public static void ThrowIfTypeMismatched<T>(
            this object AParam,
            [InvokerParameterName] string AParamName
        )
        {
            if (!AParam.IsConstrainedBy<T>())
                throw new ArgumentException(C_TypeMismatch, AParamName);
        }
    }
}

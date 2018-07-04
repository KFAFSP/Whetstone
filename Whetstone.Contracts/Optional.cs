using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using JetBrains.Annotations;

namespace Whetstone.Contracts
{
    /// <summary>
    /// Provides static factory methods for the <see cref="Optional{T}"/> type.
    /// </summary>
    [PublicAPI]
    public static class Optional
    {
        /// <summary>
        /// Returns an absent <see cref="Optional{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the absent value.</typeparam>
        /// <returns>An absent <see cref="Optional{T}"/>.</returns>
        public static Optional<T> Absent<T>() => new Optional<T>();

        /// <summary>
        /// Returns a present <see cref="Optional{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the present value.</typeparam>
        /// <param name="AValue">The value.</param>
        /// <returns>
        /// An <see cref="Optional{T}"/> wrapping the value <paramref name="AValue"/>.
        /// </returns>
        public static Optional<T> Present<T>(T AValue) => new Optional<T>(AValue);
    }

    /// <summary>
    /// Value type keeping the semantics of an optional value.
    /// </summary>
    /// <typeparam name="T">The type of the contained value.</typeparam>
    /// <remarks>
    /// The <see cref="Optional{T}"/> is an <see cref="IEnumerable{T}"/> that is either empty or
    /// contains exactly the wrapped value.
    /// </remarks>
    [PublicAPI]
    public readonly struct Optional<T> :
        IEquatable<T>,
        IEquatable<Optional<T>>,
        IEnumerable<T>
    {
        /// <summary>
        /// The absent <see cref="Optional{T}"/>.
        /// </summary>
        public static readonly Optional<T> Absent;

        const string C_NotPresent = "Optional value is not present.";

        readonly T[] FValue;

        /// <summary>
        /// Initialize an <see cref="Optional{T}"/> with a present value.
        /// </summary>
        /// <param name="AValue">The present value.</param>
        public Optional([CanBeNull] [NoEnumeration] T AValue)
        {
            FValue = new[] { AValue };
        }

        /// <summary>
        /// Check whether that present value satisfies a specified <see cref="Predicate{T}"/> and
        /// propagate it; otherwise propagate absence.
        /// </summary>
        /// <param name="APredicate">The <see cref="Predicate{T}"/> to check for.</param>
        /// <returns>
        /// An <see cref="Optional{T}"/> that wraps the matching value; otherwise absent.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="APredicate"/> is <see langword="null"/>.
        /// </exception>
        [ContractAnnotation("APredicate: null => halt;")]
        public Optional<T> If([NotNull] [InstantHandle] Predicate<T> APredicate)
        {
            if (APredicate == null) throw new ArgumentNullException(nameof(APredicate));

            return IsPresent && APredicate(FValue[0])
                ? this
                : Absent;
        }

        /// <summary>
        /// Execute an <see cref="Action{T}"/> on the value if it is present; otherwise do nothing.
        /// </summary>
        /// <remarks>
        /// Execution stops when <paramref name="AAction"/> throws any <see cref="Exception"/>,
        /// which is then propagated to the caller.
        /// </remarks>
        /// <param name="AAction">The <see cref="Action{T}"/> to execute on the value.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="AAction"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="Exception">
        /// Any <see cref="Exception"/> thrown by <paramref name="AAction"/>.
        /// </exception>
        [ContractAnnotation("AAction: null => halt;")]
        public void IfPresent([NotNull] [InstantHandle] Action<T> AAction)
        {
            if (AAction == null) throw new ArgumentNullException(nameof(AAction));

            if (IsPresent) AAction(FValue[0]);
        }

        /// <summary>
        /// Compute a <see cref="Func{T, TOut}"/> on the value if it is present and wrap the result;
        /// otherwise propagate absence.
        /// </summary>
        /// <remarks>
        /// Execution stops when <paramref name="AFunc"/> throws any <see cref="Exception"/>,
        /// which is then propagated to the caller.
        /// </remarks>
        /// <typeparam name="TOut">The type of the output.</typeparam>
        /// <param name="AFunc">The <see cref="Func{T, TOut}"/> to compute on the value.</param>
        /// <returns>
        /// An <see cref="Optional{TOut}"/> with the result present, or absent if the value is
        /// absent.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="AFunc"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="Exception">
        /// Any <see cref="Exception"/> thrown by <paramref name="AFunc"/>.
        /// </exception>
        [ContractAnnotation("AFunc: null => halt;")]
        [MustUseReturnValue]
        public Optional<TOut> IfPresent<TOut>([NotNull] [InstantHandle] Func<T, TOut> AFunc)
        {
            if (AFunc == null) throw new ArgumentNullException(nameof(AFunc));

            return IsPresent
                ? new Optional<TOut>(AFunc(FValue[0]))
                : Optional<TOut>.Absent;
        }

        /// <summary>
        /// Safely gets the value if present; otherwise returns a default.
        /// </summary>
        /// <param name="ADefault">The default value to use.</param>
        /// <returns>The value if present; otherwise <paramref name="ADefault"/>.</returns>
        [Pure]
        public T OrDefault([CanBeNull] [NoEnumeration] T ADefault = default(T))
            => IsPresent ? FValue[0] : ADefault;

        /// <summary>
        /// Unpack the present value or throw an <see cref="Exception"/>.
        /// </summary>
        /// <typeparam name="TException">The exception type.</typeparam>
        /// <returns>The present value.</returns>
        /// <exception cref="Exception">Value is absent.</exception>
        [DebuggerHidden]
        public T OrThrow<TException>()
            where TException : Exception, new()
        {
            if (!IsPresent)
                throw new TException();

            return FValue[0];
        }
        /// <summary>
        /// Unpack the present value or throw an <see cref="Exception"/>.
        /// </summary>
        /// <param name="AException">The exception.</param>
        /// <returns>The present value.</returns>
        /// <exception cref="Exception">Value is absent.</exception>
        [DebuggerHidden]
        public T OrThrow([CanBeNull] Exception AException = null)
        {
            if (!IsPresent)
                throw AException ?? new InvalidOperationException(C_NotPresent);

            return FValue[0];
        }

        /// <summary>
        /// Performs an <see langword="as"/> cast of the value if present; otherwise propagates
        /// absence.
        /// </summary>
        /// <typeparam name="TOut">The type to cast to.</typeparam>
        /// <returns>
        /// An <see cref="Optional{TOut}"/> with the cast result present, or absent if the value is
        /// absent.
        /// </returns>
        [Pure]
        public Optional<TOut> As<TOut>()
            where TOut : class
            => IsPresent
                ? new Optional<TOut>(FValue[0] as TOut)
                : Optional<TOut>.Absent;

        /// <summary>
        /// Performs an <see langword="as"/> cast of the value if present and the result will be
        /// non-<see langword="null"/>; otherwise propagates absence.
        /// </summary>
        /// <typeparam name="TOut">The type to cast to.</typeparam>
        /// <returns>
        /// An <see cref="Optional{TOut}"/> with the non-<see langword="null"/> cast result of the
        /// present value; otherwise absent.
        /// </returns>
        [Pure]
        public Optional<TOut> Is<TOut>()
            where TOut : class
        {
            if (!IsPresent)
                return Optional<TOut>.Absent;

            if (FValue[0] is TOut casted)
                return new Optional<TOut>(casted);

            return Optional<TOut>.Absent;
        }

        #region IEnumerable<T>
        /// <summary>
        /// Get an <see cref="IEnumerator{T}"/> that will yield the value if present, or nothing
        /// if absent.
        /// </summary>
        /// <returns>A new <see cref="IEnumerator{T}"/> instance.</returns>
        [Pure]
        public IEnumerator<T> GetEnumerator()
        {
            if (!IsPresent) yield break;
            yield return FValue[0];
        }

        [Pure]
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        #endregion

        #region IEquatable<T>
        /// <summary>
        /// Check whether the value is present and equal to the specified one, using default
        /// equality comparison.
        /// </summary>
        /// <param name="AValue">The value to compare with.</param>
        /// <returns>
        /// <see langword="true"/> if <see cref="IsPresent"/> is <see langword="true"/> and
        /// <see cref="Value"/> equals <paramref name="AValue"/>; otherwise <see langword="false"/>.
        /// </returns>
        [Pure]
        public bool Equals([NoEnumeration] T AValue) => IsPresent && FValue[0].Equals(AValue);
        #endregion

        #region IEquatable<Optional<T>>
        /// <summary>
        /// Check whether two <see cref="Optional{T}"/> are the equal by comparing their values if
        /// present.
        /// </summary>
        /// <param name="AOptional">The <see cref="Optional{T}"/> to compare with.</param>
        /// <returns>
        /// <see langword="true"/> if either both optionals are absent or contain the same value;
        /// otherwise <see langword="false"/>.
        /// </returns>
        [Pure]
        public bool Equals(Optional<T> AOptional)
            => IsPresent
                ? AOptional.IsPresent && FValue[0].Equals(AOptional.FValue[0])
                : !AOptional.IsPresent;
        #endregion

        #region System.Object overrides
        /// <summary>
        /// Check whether this <see cref="Optional{T}"/> is equal to the specified
        /// <see see="object"/>.
        /// </summary>
        /// <param name="AObject">The <see cref="object"/> to compare with.</param>
        /// <returns>
        /// <see langword="true"/> if any of the previously defined equality relations matches;
        /// otherwise <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// Specifically, this method returns <see langword="true"/> only if either:
        /// <list type="bullet">
        /// <item><description>
        /// <paramref name="AObject"/> is another <see cref="Optional{T}"/> and
        /// <see cref="Optional{T}.Equals(Optional{T})"/> returns <see langword="true"/>.
        /// </description></item>
        /// <item><description>
        /// <paramref name="AObject"/> is a value of type <typeparamref name="T"/> and
        /// <see cref="Optional{T}.Equals(T)"/> returns <see langword="true"/>.
        /// </description></item>
        /// </list>
        /// </remarks>
        [Pure]
        public override bool Equals(object AObject)
        {
            switch (AObject)
            {
                case Optional<T> value:
                    return Equals(value);

                case T value:
                    return Equals(value);

                default:
                    return false;
            }
        }

        /// <summary>
        /// Compute a hash code for this <see cref="Optional{T}"/>.
        /// </summary>
        /// <returns>The hash code of the present value, or <c>0</c> if absent.</returns>
        [Pure]
        public override int GetHashCode()
            => IsPresent
                ? FValue[0].GetHashCode()
                : 0;

        /// <summary>
        /// Get a string that represents this <see cref="Optional{T}"/>.
        /// </summary>
        /// <returns>
        /// Either the <see cref="object.ToString()"/> result of the present value, or an
        /// "Optional.Absent&lt;<typeparamref name="T"/>&gt;" literal if absent.
        /// </returns>
        [Pure]
        public override string ToString()
            => IsPresent
                ? FValue[0].ToString()
                : $"Optional.Absent<{typeof(T).Name}>";
        #endregion

        /// <summary>
        /// Gets a value indicating whether the optional value is present.
        /// </summary>
        public bool IsPresent => FValue != null;
        /// <summary>
        /// Gets the value if present; otherwise throws an <see cref="InvalidOperationException"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// <see cref="IsPresent"/> is <see langword="false"/>.
        /// </exception>
        public T Value => IsPresent ? FValue[0] : throw new InvalidOperationException(C_NotPresent);

        /// <summary>
        /// Implicitly unwraps the contents of an <see cref="Optional{T}"/>.
        /// </summary>
        /// <param name="AOptional">The <see cref="Optional{T}"/> to unwrap.</param>
        /// <exception cref="InvalidOperationException">The value is not present.</exception>
        [Pure]
        public static implicit operator T(Optional<T> AOptional) => AOptional.Value;
        /// <summary>
        /// Implicitly initializes a present <see cref="Optional{T}"/>.
        /// </summary>
        /// <param name="AValue">The value that is present.</param>
        [Pure]
        public static implicit operator Optional<T>(T AValue) => new Optional<T>(AValue);

        /// <summary>
        /// Return the first <see cref="Optional{T}"/> that is present.
        /// </summary>
        /// <param name="ALeft">The left <see cref="Optional{T}"/>.</param>
        /// <param name="ARight">The right <see cref="Optional{T}"/>.</param>
        /// <returns>
        /// The first <see cref="Optional{T}"/> that is present, or absent if both are absent.
        /// </returns>
        [Pure]
        public static Optional<T> operator |(Optional<T> ALeft, Optional<T> ARight)
            => ALeft.IsPresent ? ALeft : ARight;
        /// <summary>
        /// Shorthand for <see cref="Optional{T}.OrDefault(T)"/>.
        /// </summary>
        /// <param name="ALeft">The <see cref="Optional{T}"/>.</param>
        /// <param name="ARight">The default value.</param>
        /// <returns>
        /// The value of <paramref name="ALeft"/> if present; otherwise <paramref name="ARight"/>.
        /// </returns>
        [Pure]
        public static T operator |(Optional<T> ALeft, [NoEnumeration] T ARight)
            => ALeft.OrDefault(ARight);

        /// <summary>
        /// Shorthand for calling <see cref="Optional{T}.Equals(Optional{T})"/>.
        /// </summary>
        /// <param name="ALeft">The left <see cref="Optional{T}"/>.</param>
        /// <param name="ARight">The right <see cref="Optional{T}"/>.</param>
        /// <returns>
        /// <see langword="true"/> if the two <see cref="Optional{T}"/>s are equal;
        /// otherwise <see langword="false"/>.
        /// </returns>
        public static bool operator ==(Optional<T> ALeft, Optional<T> ARight)
            => ALeft.Equals(ARight);
        /// <summary>
        /// Shorthand for calling <see cref="Optional{T}.Equals(Optional{T})"/> and inverting.
        /// </summary>
        /// <param name="ALeft">The left <see cref="Optional{T}"/>.</param>
        /// <param name="ARight">The right <see cref="Optional{T}"/>.</param>
        /// <returns>
        /// <see langword="true"/> if the two <see cref="Optional{T}"/>s are inequal;
        /// otherwise <see langword="false"/>.
        /// </returns>
        public static bool operator !=(Optional<T> ALeft, Optional<T> ARight)
            => !ALeft.Equals(ARight);

        /// <summary>
        /// Shorthand for calling <see cref="Optional{T}.Equals(T)"/>.
        /// </summary>
        /// <param name="ALeft">The <see cref="Optional{T}"/>.</param>
        /// <param name="ARight">The value.</param>
        /// <returns>
        /// <see langword="true"/> if the <see cref="Optional{T}"/> and the value are equal;
        /// otherwise <see langword="false"/>.
        /// </returns>
        public static bool operator ==(Optional<T> ALeft, T ARight) => ALeft.Equals(ARight);
        /// <summary>
        /// Shorthand for calling <see cref="Optional{T}.Equals(T)"/> and inverting.
        /// </summary>
        /// <param name="ALeft">The <see cref="Optional{T}"/>.</param>
        /// <param name="ARight">The value.</param>
        /// <returns>
        /// <see langword="true"/> if the <see cref="Optional{T}"/> and the value are inequal;
        /// otherwise <see langword="false"/>.
        /// </returns>
        public static bool operator !=(Optional<T> ALeft, T ARight) => !ALeft.Equals(ARight);
    }

    /// <summary>
    /// Provides some extension methods for use with <see cref="Optional{T}"/> type.
    /// </summary>
    [PublicAPI]
    public static class OptionalExtensions
    {
        /// <summary>
        /// Get an <see cref="Optional{T}"/> that is present if the value is not
        /// <see langword="null"/>, or absent otherwise.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="AValue">The value.</param>
        /// <returns>
        /// An <see cref="Optional{T}"/> wrapping the non-<see langword="null"/>
        /// <paramref name="AValue"/>; otherwise absent.
        /// </returns>
        [Pure]
        public static Optional<T> IfNotNull<T>([NoEnumeration] this T AValue) =>
            AValue == null ? new Optional<T>() : new Optional<T>(AValue);

        /// <summary>
        /// Get an <see cref="Optional{T}"/> that contains the first value in the
        /// <see cref="IEnumerable{T}"/>, or is absent if none found.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="AEnumerable">The <see cref="IEnumerable{T}"/>.</param>
        /// <returns>
        /// An <see cref="Optional{T}"/> wrapping the first value in <paramref name="AEnumerable"/>;
        /// or is absent if it is empty.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="AEnumerable"/> is <see langword="null"/>.
        /// </exception>
        [MustUseReturnValue]
        [ContractAnnotation("AEnumerable: null => halt;")]
        public static Optional<T> IfAny<T>(
            [NotNull] [InstantHandle] this IEnumerable<T> AEnumerable
        )
        {
            if (AEnumerable == null) throw new ArgumentNullException(nameof(AEnumerable));

            using (var enumerator = AEnumerable.GetEnumerator())
            {
                if (enumerator.MoveNext())
                    return enumerator.Current;
            }

            return Optional.Absent<T>();
        }

        /// <summary>
        /// Get an <see cref="Optional{T}"/> that contains the first value in the
        /// <see cref="IEnumerable{T}"/> that matches the predicate, or is absent if none found.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="AEnumerable">The <see cref="IEnumerable{T}"/>.</param>
        /// <param name="APredicate">The predicate <see cref="Func{T, TResult}"/> to apply.</param>
        /// <returns>
        /// An <see cref="Optional{T}"/> wrapping the first value in <paramref name="AEnumerable"/>;
        /// or is absent if it is empty.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="AEnumerable"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="APredicate"/> is <see langword="null"/>.
        /// </exception>
        [MustUseReturnValue]
        [ContractAnnotation("AEnumerable: null => halt; APredicate: null => halt;")]
        public static Optional<T> IfAny<T>(
            [NotNull] [InstantHandle] this IEnumerable<T> AEnumerable,
            [NotNull] Func<T, bool> APredicate
        ) => AEnumerable
            .Where(APredicate ?? throw new ArgumentNullException(nameof(APredicate)))
            .IfAny();

        /// <summary>
        /// Get an <see cref="Optional{T}"/> that wraps the result of calling the specified
        /// <see cref="Func{TResult}"/>, or is absent if an <see cref="Exception"/> was thrown.
        /// </summary>
        /// <typeparam name="T">The type of the return value.</typeparam>
        /// <param name="AFunc">The <see cref="Func{TResult}"/> to call.</param>
        /// <returns>
        /// An <see cref="Optional{T}"/> wrapping the call result if no exception was thrown;
        /// otherwise absent.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="AFunc"/> is <see langword="null"/>.
        /// </exception>
        [MustUseReturnValue]
        [ContractAnnotation("AFunc: null => halt;")]
        public static Optional<T> IfSucceeds<T>([NotNull] [InstantHandle] this Func<T> AFunc)
        {
            if (AFunc == null) throw new ArgumentNullException(nameof(AFunc));

            try
            {
                return AFunc();
            }
            catch
            {
                return Optional.Absent<T>();
            }
        }

        /// <summary>
        /// Get an <see cref="Optional{T}"/> that wraps the result of casting to the requested type
        /// if possible, or is absent.
        /// </summary>
        /// <typeparam name="T">The type to cast to.</typeparam>
        /// <param name="AObject">The value to cast.</param>
        /// <returns>
        /// An <see cref="Optional{T}"/> wrapping the cast result if possible; otherwise absent.
        /// </returns>
        [Pure]
        public static Optional<T> IfIs<T>(this object AObject)
        {
            return AObject is T tCasted ? new Optional<T>(tCasted) : new Optional<T>();
        }

        /// <summary>
        /// Get an <see cref="Optional{T}"/> that wraps the input value if it satisfies a specified
        /// <see cref="Predicate{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="AValue">The value.</param>
        /// <param name="APredicate">The <see cref="Predicate{T}"/> to check for.</param>
        /// <returns>
        /// An <see cref="Optional{T}"/> wrapping the value if it matched the predicate; otherwise
        /// absent.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="APredicate"/> is <see langword="null"/>.
        /// </exception>
        [MustUseReturnValue]
        [ContractAnnotation("APredicate: null => halt")]
        public static Optional<T> If<T>(
            this T AValue,
            [NotNull] [InstantHandle] Predicate<T> APredicate
        )
        {
            if (APredicate == null) throw new ArgumentNullException(nameof(APredicate));

            return APredicate(AValue) ? new Optional<T>(AValue) : new Optional<T>();
        }
    }
}
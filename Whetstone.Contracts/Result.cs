﻿using System;
using System.Collections.Generic;
using System.Diagnostics;

using JetBrains.Annotations;

namespace Whetstone.Contracts
{
    /// <summary>
    /// Provides static factory methods for the <see cref="Result{T}"/> type.
    /// </summary>
    [PublicAPI]
    public static class Result
    {
        const string C_Uninitialized = "Result is uninitialized.";

        /// <summary>
        /// Get an <see cref="Exception"/> contained in an uninitialized <see cref="Result{T}"/>.
        /// </summary>
        public static Exception UninitializedError => new Exception(C_Uninitialized);

        /// <summary>
        /// Make a successful <see cref="Result{T}"/> wrapping the specified value.
        /// </summary>
        /// <typeparam name="T">The result value type.</typeparam>
        /// <param name="AValue">The successful value.</param>
        /// <returns>
        /// A successful <see cref="Result{T}"/> wrapping <paramref name="AValue"/>.
        /// </returns>
        public static Result<T> Ok<T>(T AValue) => new Result<T>(AValue);

        /// <summary>
        /// Make an erroneous <see cref="Result{T}"/> wrapping the specified error.
        /// </summary>
        /// <typeparam name="T">The result value type.</typeparam>
        /// <param name="AError">The contained error.</param>
        /// <returns>
        /// An erroneous <see cref="Result{T}"/> wrapping <paramref name="AError"/>.
        /// </returns>
        [ContractAnnotation("AError: null => halt;")]
        public static Result<T> Fail<T>([NotNull] Exception AError) => new Result<T>(AError);
    }

    /// <summary>
    /// Value type keeping the semantics of a no-throw function that maybe returns a value or an
    /// error.
    /// </summary>
    /// <typeparam name="T">The type of the contained value.</typeparam>
    /// <remarks>
    /// For simplicity's sake the implementation of <see cref="Result{T}"/> assumes that
    /// <typeparamref name="T"/> is never <see cref="Exception"/> or derived from it.
    /// </remarks>
    [PublicAPI]
    public readonly struct Result<T> :
        IEquatable<Result<T>>,
        IEquatable<T>
        // cannot implement IEquatable<Exception>
    {
        /// <summary>
        /// The uninitialized <see cref="Result{T}"/>.
        /// </summary>
        public static Result<T> Uninitialized;

        readonly object FValue;
        T UnboxValue => ((T[])FValue)[0];
        [NotNull]
        Exception UnboxError => (Exception)FValue ?? Result.UninitializedError;

        /// <summary>
        /// Initialize a successful <see cref="Result{T}"/>.
        /// </summary>
        /// <param name="AValue">The successful result value.</param>
        public Result([CanBeNull] [NoEnumeration] T AValue)
        {
            FValue = new [] { AValue };
            IsSuccess = true;
        }
        /// <summary>
        /// Initialize an erroneous <see cref="Result{T}"/>.
        /// </summary>
        /// <param name="AError">The error.</param>
        public Result([NotNull] Exception AError)
        {
            FValue = AError ?? throw new ArgumentNullException(nameof(AError));
            IsSuccess = false;
        }

        /// <summary>
        /// Execute an <see cref="Action{T}"/> on the successful value (if any).
        /// </summary>
        /// <param name="AAction">The <see cref="Action{T}"/>.</param>
        /// <returns>This <see cref="Result{T}"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="AAction"/> is <see langword="null"/>.
        /// </exception>
        [ContractAnnotation("AAction: null => halt;")]
        public Result<T> OnSuccess([NotNull] [InstantHandle] Action<T> AAction)
        {
            if (AAction == null) throw new ArgumentNullException(nameof(AAction));

            if (IsSuccess)
                AAction(UnboxValue);
            return this;
        }

        /// <summary>
        /// Execute an <see cref="Action{T}"/> on the contained matching error (if any).
        /// </summary>
        /// <typeparam name="TException">The type of expected error.</typeparam>
        /// <param name="AAction">The <see cref="Action{T}"/>.</param>
        /// <returns>This <see cref="Result{T}"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="AAction"/> is <see langword="null"/>.
        /// </exception>
        [ContractAnnotation("AAction: null => halt;")]
        public Result<T> OnError<TException>([NotNull] [InstantHandle] Action<TException> AAction)
            where TException : Exception
        {
            if (AAction == null) throw new ArgumentNullException(nameof(AAction));

            if (!IsSuccess && UnboxError is TException error)
                AAction(error);

            return this;
        }
        /// <summary>
        /// Execute an <see cref="Action{T}"/> on the contained error (if any).
        /// </summary>
        /// <param name="AAction">The <see cref="Action{T}"/>.</param>
        /// <returns>This <see cref="Result{T}"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="AAction"/> is <see langword="null"/>.
        /// </exception>
        [ContractAnnotation("AAction: null => halt;")]
        public Result<T> OnError([NotNull] [InstantHandle] Action<Exception> AAction)
            => OnError<Exception>(AAction);

        /// <summary>
        /// Compute a <see cref="Func{T, TResult}"/> on the successful value (if any) and propagate
        /// the results.
        /// </summary>
        /// <typeparam name="TOut">The output type.</typeparam>
        /// <param name="AFunc">The <see cref="Func{T, TResult}"/>.</param>
        /// <returns>
        /// A <see cref="Result{T}"/> wrapping the result of <paramref name="AFunc"/> or propagating
        /// the contained error.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="AFunc"/> is <see langword="null"/>.
        /// </exception>
        [ContractAnnotation("AFunc: null => halt;")]
        public Result<TOut> AndThen<TOut>([NotNull] [InstantHandle] Func<T, TOut> AFunc)
        {
            if (AFunc == null) throw new ArgumentNullException(nameof(AFunc));

            return IsSuccess
                ? new Result<TOut>(AFunc(UnboxValue))
                : new Result<TOut>(UnboxError);
        }
        /// <summary>
        /// Compute a <see cref="Func{T, TResult}"/> on the successful value (if any) and propagate
        /// the results, or catch and propagate the error.
        /// </summary>
        /// <typeparam name="TOut">The output type.</typeparam>
        /// <param name="AFunc">The <see cref="Func{T, TResult}"/>.</param>
        /// <returns>
        /// A <see cref="Result{T}"/> wrapping the result of <paramref name="AFunc"/> or propagating
        /// the contained error or propagating the error during <paramref name="AFunc"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="AFunc"/> is <see langword="null"/>.
        /// </exception>
        [ContractAnnotation("AFunc: null => halt;")]
        public Result<TOut> AndThenTry<TOut>([NotNull] [InstantHandle] Func<T, TOut> AFunc)
        {
            if (AFunc == null) throw new ArgumentNullException(nameof(AFunc));

            if (!IsSuccess)
                return new Result<TOut>(UnboxError);

            try
            {
                return new Result<TOut>(AFunc(UnboxValue));
            }
            catch (Exception error)
            {
                return new Result<TOut>(error);
            }
        }

        /// <summary>
        /// Ignore the contained error (if any).
        /// </summary>
        /// <returns>An <see cref="Optional{T}"/> wrapping the successful value.</returns>
        public Optional<T> IgnoreError()
        {
            return IsSuccess
                ? new Optional<T>(UnboxValue)
                : Optional<T>.Absent;
        }

        #region IEquatable<Result<T>>
        /// <inheritdoc />
        [Pure]
        public bool Equals(Result<T> AResult)
        {
            if (IsSuccess)
            {
                return AResult.IsSuccess
                    && EqualityComparer<T>.Default.Equals(UnboxValue, AResult.UnboxValue);
            }

            return !AResult.IsSuccess
                && ReferenceEquals(UnboxError, AResult.UnboxError);
        }
        #endregion

        #region IEquatable<T>
        /// <inheritdoc />
        [Pure]
        public bool Equals(T AValue)
            => IsSuccess
                && EqualityComparer<T>.Default.Equals(UnboxValue, AValue);
        #endregion

        /// <summary>
        /// Check if this <see cref="Result{T}"/> is equal to an <see cref="Exception"/>.
        /// </summary>
        /// <param name="AError">The error.</param>
        /// <returns>
        /// <see langword="true"/> if the <see cref="Result{T}"/> is erroneous and wraps the same
        /// error object; otherwise <see langword="false"/>.
        /// </returns>
        [Pure]
        public bool Equals(Exception AError)
            => ReferenceEquals(Error, AError);

        #region System.Object overrides
        /// <summary>
        /// Check whether this <see cref="Result{T}"/> is equal to the specified
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
        /// <li>
        /// <paramref name="AObject"/> is another <see cref="Result{T}"/> and
        /// <see cref="Result{T}.Equals(Result{T})"/> returns <see langword="true"/>.
        /// </li>
        /// </list>
        /// </remarks>
        [Pure]
        public override bool Equals(object AObject)
        {
            switch (AObject)
            {
                case Result<T> result:
                    return Equals(result);

                case T value:
                    return Equals(value);

                case Exception error:
                    return Equals(error);
            }

            return false;
        }

        /// <summary>
        /// Compute a hash code for this <see cref="Result{T}"/>.
        /// </summary>
        /// <returns>The hash code of the successful value or the contained error.</returns>
        [Pure]
        public override int GetHashCode()
            => IsSuccess
                ? UnboxValue.GetHashCode()
                : UnboxError.GetHashCode();

        /// <summary>
        /// Get a string that represents this <see cref="Result{T}"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="object.ToString()"/> result of either the successful value or the
        /// "Result.Fail&lt;<typeparamref name="T"/>&gt;(Exception)" error literal.
        /// </returns>
        [Pure]
        public override string ToString()
            => IsSuccess
                ? UnboxValue.ToString()
                : $"Result.Fail<{typeof(T).Name}>({UnboxError.GetType().Name})";
        #endregion

        /// <summary>
        /// Throw the contained error if this <see cref="Result{T}"/> is erroneous.
        /// </summary>
        /// <exception cref="Exception">This result is erroneous..</exception>
        [DebuggerHidden]
        public void ThrowIfError()
        {
            if (!IsSuccess) throw UnboxError;
        }

        /// <summary>
        /// Get a value indicating whether this result indicates a success.
        /// </summary>
        public bool IsSuccess { get; }

        /// <summary>
        /// Get the successful result value or throw the contained error.
        /// </summary>
        public T Value => IsSuccess ? UnboxValue : throw UnboxError;
        /// <summary>
        /// Get the contained error, or <see langword="null"/> if successful.
        /// </summary>
        [CanBeNull]
        public Exception Error => IsSuccess ? null : UnboxError;

        /// <summary>
        /// Implicitly unwraps the contents of a <see cref="Result{T}"/>.
        /// </summary>
        /// <param name="AResult">The <see cref="Result{T}"/> to unwrap.</param>
        /// <exception cref="Exception">The result is erroneous.</exception>
        [Pure]
        public static implicit operator T(Result<T> AResult) => AResult.Value;
        /// <summary>
        /// Implicitly initializes a successful <see cref="Result{T}"/>.
        /// </summary>
        /// <param name="AValue">The success value.</param>
        [Pure]
        public static implicit operator Result<T>(T AValue) => new Result<T>(AValue);

        /// <summary>
        /// Implicitly unwraps the error of a <see cref="Result{T}"/>.
        /// </summary>
        /// <param name="AResult">The <see cref="Result{T}"/> to unwrap.</param>
        [Pure]
        public static implicit operator Exception(Result<T> AResult) => AResult.Error;
        /// <summary>
        /// Implicitly initializes an erroneous <see cref="Result{T}"/>.
        /// </summary>
        /// <param name="AError">The error.</param>
        [Pure]
        [ContractAnnotation("AError: null => halt;")]
        public static implicit operator Result<T>([NotNull] Exception AError)
            => new Result<T>(AError);

        /// <summary>
        /// Shorthand for calling <see cref="Result{T}.Equals(Result{T})"/>.
        /// </summary>
        /// <param name="ALeft">The left <see cref="Result{T}"/>.</param>
        /// <param name="ARight">The right <see cref="Result{T}"/>.</param>
        /// <returns>
        /// <see langword="true"/> if the two <see cref="Result{T}"/>s are equal;
        /// otherwise <see langword="false"/>.
        /// </returns>
        public static bool operator ==(Result<T> ALeft, Result<T> ARight)
            => ALeft.Equals(ARight);
        /// <summary>
        /// Shorthand for calling <see cref="Result{T}.Equals(Result{T})"/> and inverting.
        /// </summary>
        /// <param name="ALeft">The left <see cref="Optional{T}"/>.</param>
        /// <param name="ARight">The right <see cref="Optional{T}"/>.</param>
        /// <returns>
        /// <see langword="true"/> if the two <see cref="Optional{T}"/>s are inequal;
        /// otherwise <see langword="false"/>.
        /// </returns>
        public static bool operator !=(Result<T> ALeft, Result<T> ARight)
            => !ALeft.Equals(ARight);

        /// <summary>
        /// Shorthand for calling <see cref="Result{T}.Equals(T)"/>.
        /// </summary>
        /// <param name="ALeft">The <see cref="Result{T}"/>.</param>
        /// <param name="ARight">The value.</param>
        /// <returns>
        /// <see langword="true"/> if the <see cref="Result{T}"/> and the value are equal;
        /// otherwise <see langword="false"/>.
        /// </returns>
        public static bool operator ==(Result<T> ALeft, T ARight) => ALeft.Equals(ARight);
        /// <summary>
        /// Shorthand for calling <see cref="Result{T}.Equals(T)"/> and inverting.
        /// </summary>
        /// <param name="ALeft">The <see cref="Result{T}"/>.</param>
        /// <param name="ARight">The value.</param>
        /// <returns>
        /// <see langword="true"/> if the <see cref="Result{T}"/> and the value are inequal;
        /// otherwise <see langword="false"/>.
        /// </returns>
        public static bool operator !=(Result<T> ALeft, T ARight) => !ALeft.Equals(ARight);

        /// <summary>
        /// Shorthand for calling <see cref="Result{T}.Equals(Exception)"/>.
        /// </summary>
        /// <param name="ALeft">The <see cref="Result{T}"/>.</param>
        /// <param name="ARight">The value.</param>
        /// <returns>
        /// <see langword="true"/> if the <see cref="Result{T}"/> and the error are equal;
        /// otherwise <see langword="false"/>.
        /// </returns>
        public static bool operator ==(Result<T> ALeft, Exception ARight) => ALeft.Equals(ARight);
        /// <summary>
        /// Shorthand for calling <see cref="Result{T}.Equals(Exception)"/> and inverting.
        /// </summary>
        /// <param name="ALeft">The <see cref="Result{T}"/>.</param>
        /// <param name="ARight">The value.</param>
        /// <returns>
        /// <see langword="true"/> if the <see cref="Result{T}"/> and the error are inequal;
        /// otherwise <see langword="false"/>.
        /// </returns>
        public static bool operator !=(Result<T> ALeft, Exception ARight) => !ALeft.Equals(ARight);
    }

    /// <summary>
    /// Provides some extension methods for use with <see cref="Result{T}"/> type.
    /// </summary>
    [PublicAPI]
    public static class ResultExtensions
    {
        /// <summary>
        /// Try to compute a <see cref="Func{TResult}"/> and wrap it's result.
        /// </summary>
        /// <typeparam name="T">The result type.</typeparam>
        /// <param name="AFunc">The <see cref="Func{TResult}"/>.</param>
        /// <returns>
        /// A <see cref="Result{T}"/> wrapping the result or any <see cref="Exception"/> thrown by
        /// <paramref name="AFunc"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="AFunc"/> is <see langword="null"/>.
        /// </exception>
        public static Result<T> Try<T>([NotNull] [InstantHandle] this Func<T> AFunc)
        {
            if (AFunc == null) throw new ArgumentNullException(nameof(AFunc));

            try
            {
                return new Result<T>(AFunc());
            }
            catch (Exception error)
            {
                return new Result<T>(error);
            }
        }
    }
}
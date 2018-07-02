using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JetBrains.Annotations;

namespace Whetstone.Contracts
{
    /// <summary>
    ///
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
    ///
    /// </summary>
    /// <typeparam name="T"></typeparam>
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
    }

    /// <summary>
    /// Provides some extension methods for use with <see cref="Result{T}"/> type.
    /// </summary>
    [PublicAPI]
    public static class ResultExtensions
    {

    }
}

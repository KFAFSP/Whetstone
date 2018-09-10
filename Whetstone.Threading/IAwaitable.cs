using System.Threading;
using System.Threading.Tasks;

using JetBrains.Annotations;

namespace Whetstone.Threading
{
    /// <summary>
    /// Represents an object that provides an awaitable event.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The side effects and definition of what exactly is awaitable are not defined by this
    /// interface.
    /// </para>
    /// <para>
    /// This interface is the value-less version of <see cref="IAwaitable{TResult}"/>.
    /// </para>
    /// </remarks>
    [PublicAPI]
    public interface IAwaitable
    {
        /// <summary>
        /// Wait for the event.
        /// </summary>
        /// <param name="ACancel">A <see cref="CancellationToken"/> to cancel the wait.</param>
        /// <returns>
        /// An awaitable, cancellable <see cref="Task"/> that waits for the event.
        /// </returns>
        [NotNull]
        Task WaitAsync(CancellationToken ACancel);
    }

    /// <summary>
    /// Represents an object that provides an awaitable result.
    /// </summary>
    /// <typeparam name="TResult">The awaitable result type.</typeparam>
    /// <remarks>
    /// <para>
    /// The side effects and definition of what exactly is awaitable are not defined by this
    /// interface.
    /// </para>
    /// <para>
    /// If the future should define no value, consider using <see cref="IAwaitable"/> instead.
    /// </para>
    /// </remarks>
    [PublicAPI]
    public interface IAwaitable<TResult>
    {
        /// <summary>
        /// Wait for the result.
        /// </summary>
        /// <param name="ACancel">A <see cref="CancellationToken"/> to cancel the wait.</param>
        /// <returns>
        /// An awaitable, cancellable <see cref="Task{TResult}"/> that waits for the value.
        /// </returns>
        [NotNull]
        Task<TResult> WaitAsync(CancellationToken ACancel);
    }
}

using System.Threading.Tasks;

namespace Whetstone.Threading
{
    /// <summary>
    /// Internal helper type to indicate value-less <see cref="TaskCompletionSource{TResult}"/>s.
    /// </summary>
    internal struct Void
    {
        // Because this type is declared internal it is impossible to pass it over the public API.
    }
}
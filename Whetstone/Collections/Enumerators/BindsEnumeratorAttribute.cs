using System;
using System.Collections.Generic;

namespace Whetstone.Collections.Enumerators
{
    /// <summary>
    /// <see cref="Attribute"/> indicating that an operation will bind an
    /// <see cref="IEnumerator{T}"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Parameter)]
    public sealed class BindsEnumeratorAttribute : Attribute
    { }
}

using System;

namespace Velo
{
    /// <summary>
    /// Lazy get <see cref="Value"/> from source
    /// </summary>
    /// <typeparam name="T">Type of instance</typeparam>
    public interface IReference<out T> : IDisposable
    {
        T Value { get; }
    }
}
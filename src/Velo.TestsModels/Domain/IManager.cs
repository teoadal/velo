using System;

namespace Velo.TestsModels.Domain
{
    public interface IManager<T> : IDisposable
    {
        bool Disposed { get; }

        void Do(T data);
    }
}
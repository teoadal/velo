using System;

namespace Velo.TestsModels.Domain
{
    public interface IManager<in T> : IDisposable
    {
        bool Disposed { get; }

        void Do(T data);
    }
}
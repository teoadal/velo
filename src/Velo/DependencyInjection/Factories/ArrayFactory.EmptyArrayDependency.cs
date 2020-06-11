using System;
using System.Diagnostics;
using Velo.DependencyInjection.Dependencies;
using Velo.Utils;

namespace Velo.DependencyInjection.Factories
{
    internal sealed partial class ArrayFactory
    {
        [DebuggerDisplay("Empty array of {typeof(T).Name}")]
        private sealed class EmptyArrayDependency<T> : IDependency
        {
            public Type[] Contracts => _contracts ??= new[] {Implementation};

            public Type Implementation { get; }

            public DependencyLifetime Lifetime => DependencyLifetime.Singleton;

            private Type[]? _contracts;

            public EmptyArrayDependency()
            {
                Implementation = Typeof<T[]>.Raw;
            }

            public bool Applicable(Type contract)
            {
                return contract == Implementation || contract.IsAssignableFrom(Implementation);
            }

            public object GetInstance(Type contract, IServiceProvider services)
            {
                return Array.Empty<T>();
            }

            #region Interfaces

            void IDependency.Init(IDependencyEngine engine)
            {
            }

            void IDisposable.Dispose()
            {
            }

            #endregion
        }
    }
}
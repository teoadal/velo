using System;
using System.Runtime.CompilerServices;
using Velo.DependencyInjection;
using Velo.DependencyInjection.Dependencies;

namespace Velo.ECS.Injection
{
    internal sealed class EntityContextDependency<TEntityContext> : IDependency
        where TEntityContext : class
    {
        public Type[] Contracts => _contracts ??= new[] {Implementation};

        public Type Implementation { get; }

        public DependencyLifetime Lifetime => DependencyLifetime.Singleton;

        private Type[]? _contracts;
        private readonly Func<TEntityContext, object> _resolver;

        public EntityContextDependency(Type contract, Func<TEntityContext, object> resolver)
        {
            Implementation = contract;

            _resolver = resolver;
        }

        public bool Applicable(Type contract)
        {
            return Implementation == contract;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object GetInstance(Type contract, IServiceProvider services)
        {
            var entityContext = services.GetRequired<TEntityContext>();
            return _resolver(entityContext);
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
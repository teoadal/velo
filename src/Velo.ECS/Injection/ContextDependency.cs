using System;
using System.Runtime.CompilerServices;
using Velo.DependencyInjection;
using Velo.DependencyInjection.Dependencies;
using Velo.DependencyInjection.Resolvers;

namespace Velo.ECS.Injection
{
    internal sealed class ContextDependency<TContext> : DependencyResolver, IDependency
        where TContext : class
    {
        public Type[] Contracts => _contracts ??= new[] {Implementation};
        public DependencyLifetime Lifetime => DependencyLifetime.Singleton;
        public DependencyResolver Resolver => this;

        private Type[]? _contracts;
        private readonly Func<TContext, object> _resolver;

        public ContextDependency(Type contract, Func<TContext, object> resolver) : base(contract)
        {
            _resolver = resolver;
        }

        public bool Applicable(Type contract)
        {
            return Implementation == contract;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object GetInstance(Type contract, IDependencyScope scope)
        {
            var actorContext = scope.GetRequiredService<TContext>();
            return _resolver(actorContext);
        }

        protected override object ResolveInstance(Type contract, IDependencyScope scope)
        {
            return GetInstance(contract, scope);
        }

        public void Dispose()
        {
        }
    }
}
using System;
using System.Diagnostics;

namespace Velo.DependencyInjection.Resolvers
{
    [DebuggerDisplay("Implementation = {typeof(T)}")]
    internal sealed class DelegateResolver<T> : DependencyResolver
        where T : class
    {
        private readonly Func<IServiceProvider, T> _builder;

        public DelegateResolver(Func<IServiceProvider, T> builder, Type? implementation = null)
            : base(implementation ?? typeof(T))
        {
            _builder = builder;
        }

        public override void Init(DependencyLifetime lifetime, IDependencyEngine engine)
        {
        }

        protected override object ResolveInstance(Type contract, IServiceProvider services)
        {
            return _builder(services);
        }
    }

    [DebuggerDisplay("Implementation = {" + nameof(Implementation) + "}")]
    internal sealed class DelegateResolver : DependencyResolver
    {
        private readonly Func<IServiceProvider, object> _builder;

        public DelegateResolver(Type implementation, Func<IServiceProvider, object> builder) : base(implementation)
        {
            _builder = builder;
        }

        public override void Init(DependencyLifetime lifetime, IDependencyEngine engine)
        {
        }

        protected override object ResolveInstance(Type contract, IServiceProvider services)
        {
            return _builder(services);
        }
    }
}
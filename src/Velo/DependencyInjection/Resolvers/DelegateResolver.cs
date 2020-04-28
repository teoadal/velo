using System;
using System.Diagnostics;

namespace Velo.DependencyInjection.Resolvers
{
    [DebuggerDisplay("Implementation = {typeof(T)}")]
    internal sealed class DelegateResolver<T> : DependencyResolver
        where T : class
    {
        private readonly Func<IDependencyScope, T> _builder;

        public DelegateResolver(Func<IDependencyScope, T> builder, Type? implementation = null)
            : base(implementation ?? typeof(T))
        {
            _builder = builder;
        }

        protected override object ResolveInstance(Type contract, IDependencyScope scope)
        {
            return _builder(scope);
        }
    }
    
    [DebuggerDisplay("Implementation = {" + nameof(Implementation) + "}")]
    internal sealed class DelegateResolver : DependencyResolver
    {
        private readonly Func<IServiceProvider, object> _builder;

        public DelegateResolver(Type implementation, Func<IServiceProvider, object> builder): base(implementation)
        {
            _builder = builder;
        }

        protected override object ResolveInstance(Type contract, IDependencyScope scope)
        {
            return _builder(scope);
        }
    }
}
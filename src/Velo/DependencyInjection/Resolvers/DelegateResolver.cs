using System;
using System.Diagnostics;

namespace Velo.DependencyInjection.Resolvers
{
    [DebuggerDisplay("Implementation = {typeof(T)}")]
    internal sealed class DelegateResolver<T> : DependencyResolver
        where T : class
    {
        private readonly Func<IDependencyScope, T> _builder;

        public DelegateResolver(Func<IDependencyScope, T> builder): base(typeof(T))
        {
            _builder = builder;
        }

        protected override object GetInstance(Type contract, IDependencyScope scope)
        {
            return _builder(scope);
        }
    }
}
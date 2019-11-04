using System;
using Velo.DependencyInjection.Engines;

namespace Velo.DependencyInjection.Resolvers
{
    internal sealed class DelegateResolver<TContract> : DependencyResolver
        where TContract : class
    {
        private Func<DependencyProvider, TContract> _builder;

        public DelegateResolver(Func<DependencyProvider, TContract> builder, DependencyLifetime lifetime) :
            base(null, lifetime)
        {
            _builder = builder;
        }

        public override object Resolve(DependencyProvider scope)
        {
            return _builder(scope);
        }

        protected override void Initialize(DependencyEngine engine)
        {
        }

        public override void Dispose()
        {
            _builder = null;
        }
    }
}
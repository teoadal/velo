using System;
using Velo.DependencyInjection.Engine;

namespace Velo.DependencyInjection.Resolvers
{
    internal sealed class InstanceResolver : DependencyResolver, IDisposable
    {
        private readonly object _instance;

        public InstanceResolver(object instance)
            : base(instance.GetType(), DependencyLifetime.Singleton)
        {
            _instance = instance;
        }

        public override object Resolve(DependencyProvider scope)
        {
            return _instance;
        }

        protected override void Initialize(DependencyEngine engine)
        {
        }

        public void Dispose()
        {
            if (_instance is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}
using Velo.DependencyInjection.Engines;
using Velo.Utils;

namespace Velo.DependencyInjection.Resolvers
{
    internal sealed class InstanceResolver : DependencyResolver
    {
        private object _instance;

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

        public override void Dispose()
        {
            if (ReflectionUtils.IsDisposable(_instance, out var disposable))
            {
                disposable.Dispose();
            }

            _instance = null;
        }
    }
}
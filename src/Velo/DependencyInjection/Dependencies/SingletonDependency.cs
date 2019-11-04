using Velo.DependencyInjection.Resolvers;
using Velo.Utils;

namespace Velo.DependencyInjection.Dependencies
{
    internal sealed class SingletonDependency : Dependency
    {
        private bool _disposed;
        private object _instance;

        public SingletonDependency(DependencyResolver resolver): base(resolver)
        {
        }

        public override object GetInstance(DependencyProvider scope)
        {
            if (_disposed)
            {
                throw Error.Disposed(nameof(SingletonDependency));
            }
            
            return _instance ??= Resolver.Resolve(scope);
        }

        public override void Dispose()
        {
            if (_disposed) return;
            
            if (ReflectionUtils.IsDisposable(_instance, out var disposable))
            {
                disposable.Dispose();
            }

            _instance = null;
            _disposed = true;

            base.Dispose();
        }
    }
}
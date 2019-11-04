using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Velo.DependencyInjection.Resolvers;
using Velo.Extensions;
using Velo.Utils;

namespace Velo.DependencyInjection.Dependencies
{
    internal sealed class ScopeDependency : Dependency
    {
        private bool _disposed;
        private Dictionary<DependencyProvider, object> _instances;

        public ScopeDependency(DependencyResolver resolver): base(resolver)
        {
            _instances = new Dictionary<DependencyProvider, object>();
        }

        public override object GetInstance(DependencyProvider scope)
        {
            if (_disposed)
            {
                throw Error.Disposed(nameof(ScopeDependency));
            }
            
            if (_instances.TryGetValue(scope, out var existsInstance)) return existsInstance;

            var instance = Resolve(scope);
            _instances.Add(scope, instance);

            return instance;
        }

        private void Destroy(DependencyProvider scope, object instance)
        {
            scope.Destroy -= OnScopeDestroy;

            if (ReflectionUtils.IsDisposable(instance, out var disposable))
            {
                disposable.Dispose();
            }
        }

        private void OnScopeDestroy(DependencyProvider scope)
        {
            Destroy(scope, _instances[scope]);

            _instances.Remove(scope);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private object Resolve(DependencyProvider scope)
        {
            scope.Destroy += OnScopeDestroy;
            return Resolver.Resolve(scope);
        }
        
        public override void Dispose()
        {
            if (_disposed) return;
            
            foreach (var (scope, instance) in _instances)
            {
                Destroy(scope, instance);
            }
            
            _instances.Clear();
            _instances = null;
            _disposed = true;
            
            base.Dispose();
        }
    }
}
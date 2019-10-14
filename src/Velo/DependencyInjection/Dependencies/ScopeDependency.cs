using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Velo.DependencyInjection.Resolvers;
using Velo.Extensions;

namespace Velo.DependencyInjection.Dependencies
{
    internal sealed class ScopeDependency : Dependency, IDisposable
    {
        private readonly Dictionary<DependencyProvider, object> _instances;

        public ScopeDependency(DependencyResolver resolver): base(resolver)
        {
            _instances = new Dictionary<DependencyProvider, object>();
        }

        public override object GetInstance(DependencyProvider scope)
        {
            if (_instances.TryGetValue(scope, out var existsInstance)) return existsInstance;

            var instance = Resolve(scope);
            _instances.Add(scope, instance);

            return instance;
        }

        public void Dispose()
        {
            foreach (var (scope, instance) in _instances)
            {
                Destroy(scope, instance);
            }
        }

        private void Destroy(DependencyProvider scope, object instance)
        {
            scope.Destroy -= OnScopeDestroy;

            if (instance != null && instance is IDisposable disposable)
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
    }
}
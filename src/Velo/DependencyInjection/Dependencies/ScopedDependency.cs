using System;
using System.Collections.Generic;
using System.Diagnostics;
using Velo.DependencyInjection.Resolvers;
using Velo.Extensions;
using Velo.Utils;

namespace Velo.DependencyInjection.Dependencies
{
    [DebuggerDisplay("Contract = {Contracts[0]}")]
    internal sealed class ScopedDependency : Dependency
    {
        private readonly Dictionary<IDependencyScope, object> _instances;

        public ScopedDependency(Type[] contracts, DependencyResolver resolver)
            : base(contracts, resolver, DependencyLifetime.Scoped)
        {
            _instances = new Dictionary<IDependencyScope, object>();
        }

        public override object GetInstance(Type contract, IDependencyScope scope)
        {
            if (_instances.TryGetValue(scope, out var existsInstance)) return existsInstance;

            var instance = Resolve(contract, scope);
            _instances.Add(scope, instance);
            scope.Destroy += OnScopeDestroy;

            return instance;
        }

        private void OnScopeDestroy(IDependencyScope scope)
        {
            if (!_instances.TryGetValue(scope, out var instance)) return;

            _instances.Remove(scope);
            scope.Destroy -= OnScopeDestroy;

            if (ReflectionUtils.IsDisposable(instance, out var disposable))
            {
                disposable.Dispose();
            }
        }

        public override void Dispose()
        {
            foreach (var (scope, instance) in _instances)
            {
                scope.Destroy -= OnScopeDestroy;

                if (ReflectionUtils.IsDisposable(instance, out var disposable))
                {
                    disposable.Dispose();
                }
            }

            _instances.Clear();
        }
    }
}
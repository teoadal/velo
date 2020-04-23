using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Velo.Utils;

namespace Velo.DependencyInjection
{
    public sealed class DependencyProvider : IDependencyScope
    {
        public event Action<IDependencyScope>? Destroy;

        private bool _disposed;
        private readonly IDependencyEngine? _engine;
        private readonly object _lock;
        private readonly DependencyProvider? _parent;

        internal DependencyProvider(IDependencyEngine engine)
        {
            _engine = engine;
            _lock = new object();
        }

        private DependencyProvider(DependencyProvider parent)
        {
            _parent = parent;
            _lock = parent._lock;
        }

        public IDependencyScope CreateScope()
        {
            return new DependencyProvider(this);
        }

        public object Activate(Type implementation, ConstructorInfo? constructor = null)
        {
            if (implementation.IsInterface || implementation.IsGenericTypeDefinition)
            {
                throw Error.InvalidOperation($"Type {ReflectionUtils.GetName(implementation)} can't be activated");
            }

            if (_disposed) throw Error.Disposed(nameof(DependencyProvider));

            constructor ??= ReflectionUtils.GetConstructor(implementation);

            if (constructor == null)
            {
                throw Error.DefaultConstructorNotFound(implementation);
            }

            var engine = GetEngine();
            var parameters = constructor.GetParameters();

            var parameterInstances = new object?[parameters.Length];

            lock (_lock)
            {
                for (var i = parameters.Length - 1; i >= 0; i--)
                {
                    var parameter = parameters[i];
                    var parameterType = parameter.ParameterType;
                    var required = !parameter.HasDefaultValue;

                    var dependency = required 
                        ? engine.GetRequiredDependency(parameterType)
                        : engine.GetDependency(parameterType);

                    parameterInstances[i] = dependency?.GetInstance(parameterType, this);
                }
            }

            try
            {
                return constructor.Invoke(parameterInstances);
            }
            catch (TargetInvocationException e)
            {
                throw e.InnerException ?? e;
            }
        }

        public object GetRequiredService(Type contract)
        {
            if (_disposed) throw Error.Disposed(nameof(DependencyProvider));

            var engine = GetEngine();

            lock (_lock)
            {
                var dependency = engine.GetRequiredDependency(contract);
                return dependency.GetInstance(contract, this);
            }
        }

        public object? GetService(Type contract)
        {
            if (_disposed) throw Error.Disposed(nameof(DependencyProvider));
            var engine = GetEngine();

            lock (_lock)
            {
                var dependency = engine.GetDependency(contract);
                return dependency?.GetInstance(contract, this);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private IDependencyEngine GetEngine()
        {
            if (_engine != null) return _engine;
            return _parent == null
                ? throw Error.InvalidOperation("Bad dependency provider configuration")
                : _parent.GetEngine();
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true; // contains self

            var evt = Destroy;
            evt?.Invoke(this);

            _engine?.Dispose();
        }
    }
}
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Velo.Utils;

namespace Velo.DependencyInjection
{
    public sealed class DependencyProvider : IDependencyScope, IServiceProvider
    {
        public event Action<IDependencyScope> Destroy;

        private bool _disposed;
        private readonly DependencyEngine _engine;
        private readonly object _lock;
        private readonly DependencyProvider _parent;

        internal DependencyProvider(DependencyEngine engine)
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

        public T Activate<T>(ConstructorInfo constructor = null)
        {
            return (T) Activate(typeof(T), constructor);
        }

        public object Activate(Type implementation, ConstructorInfo constructor = null)
        {
            if (implementation.IsInterface || implementation.IsGenericTypeDefinition)
            {
                throw Error.InvalidOperation($"Type {ReflectionUtils.GetName(implementation)} can't be activated");
            }

            if (_disposed) throw Error.Disposed(nameof(DependencyProvider));

            if (constructor == null)
            {
                constructor = ReflectionUtils.GetConstructor(implementation);
            }

            var engine = GetEngine();
            var parameters = constructor.GetParameters();

            var parameterInstances = new object[parameters.Length];

            lock (_lock)
            {
                for (var i = 0; i < parameters.Length; i++)
                {
                    var parameter = parameters[i];
                    var parameterType = parameter.ParameterType;
                    var required = !parameter.HasDefaultValue;

                    var dependency = engine.GetDependency(parameterType, required);
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
                var dependency = engine.GetDependency(contract, true);
                return dependency.GetInstance(contract, this);
            }
        }

        public T GetRequiredService<T>() => (T) GetRequiredService(Typeof<T>.Raw);

        public T GetService<T>() => (T) GetService(Typeof<T>.Raw);

        public object GetService(Type contract)
        {
            if (_disposed) throw Error.Disposed(nameof(DependencyProvider));
            var engine = GetEngine();

            lock (_lock)
            {
                var dependency = engine.GetDependency(contract);
                return dependency?.GetInstance(contract, this);
            }
        }

        public T[] GetServices<T>() => (T[]) GetService(Typeof<T[]>.Raw);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private DependencyEngine GetEngine()
        {
            return _engine ?? _parent.GetEngine();
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
using System;
using System.Diagnostics;
using Velo.Utils;

namespace Velo.Dependencies.Singletons
{
    [DebuggerDisplay("Singleton {_type.Name}")]
    internal sealed class BuilderSingleton<T> : Dependency
        where T : class
    {
        private readonly Func<DependencyContainer, T> _builder;
        private readonly bool _isDisposable;
        private readonly Type _type;

        private T _instance;

        public BuilderSingleton(Type[] contracts, Func<DependencyContainer, T> builder) : base(contracts)
        {
            _builder = builder;
            _type = typeof(T);
            _isDisposable = ReflectionUtils.IsDisposableType(_type);
        }

        public override void Destroy()
        {
            if (_isDisposable)
            {
                ((IDisposable) _instance)?.Dispose();
            }

            _instance = null;
        }

        public override object Resolve(Type contract, DependencyContainer container)
        {
            if (_instance != null) return _instance;

            _instance = _builder(container);
            return _instance;
        }
    }
}
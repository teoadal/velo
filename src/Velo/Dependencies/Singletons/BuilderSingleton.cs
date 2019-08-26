using System;

namespace Velo.Dependencies.Singletons
{
    internal sealed class BuilderSingleton<T> : Dependency
        where T : class
    {
        private readonly Func<DependencyContainer, T> _builder;
        private readonly bool _isDisposable;

        private T _instance;

        public BuilderSingleton(Type[] contracts, Func<DependencyContainer, T> builder)
            : base(contracts)
        {
            _builder = builder;
            _isDisposable = typeof(T).IsAssignableFrom(typeof(IDisposable));
        }

        public override void Destroy()
        {
            if (_isDisposable)
            {
                ((IDisposable) _instance)?.Dispose();
            }

            _instance = null;
        }

        public override object Resolve(Type requestedType, DependencyContainer container)
        {
            if (_instance != null) return _instance;

            _instance = _builder(container);
            return _instance;
        }
    }
}
using System;

namespace Velo.Dependencies.Singletons
{
    internal sealed class BuilderSingleton<T> : Dependency
        where T : class
    {
        private readonly Func<DependencyContainer, T> _builder;
        private bool _isDisposable;

        private T _instance;

        public BuilderSingleton(Type[] contracts, Func<DependencyContainer, T> builder) : base(contracts)
        {
            _builder = builder;
        }

        public override void Destroy()
        {
            if (!_isDisposable) return;

            ((IDisposable) _instance)?.Dispose();
            _isDisposable = false;
        }

        public override object Resolve(Type requestedType, DependencyContainer container)
        {
            if (_instance != null) return _instance;

            _instance = _builder(container);
            _isDisposable = _instance.GetType().IsAssignableFrom(typeof(IDisposable));
            return _instance;
        }
    }
}
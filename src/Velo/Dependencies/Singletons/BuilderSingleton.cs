using System;

namespace Velo.Dependencies.Singletons
{
    internal sealed class BuilderSingleton<T> : IDependency
        where T : class
    {
        private readonly Type _contract;
        private Func<DependencyContainer, T> _builder;
        private T _instance;

        public BuilderSingleton(Type contract, Func<DependencyContainer, T> builder)
        {
            _contract = contract;
            _builder = builder;
        }

        public bool Applicable(Type requestedType)
        {
            return _contract == requestedType;
        }

        public void Destroy()
        {
            _builder = null;
            
            // ReSharper disable once InvertIf
            if (_instance != null && _instance is IDisposable disposable)
            {
                disposable.Dispose();
                _instance = null;
            }
        }

        public object Resolve(Type requestedType, DependencyContainer container)
        {
            if (_instance != null) return _instance;

            _instance = _builder(container);
            _builder = null;
            return _instance;
        }
    }
}
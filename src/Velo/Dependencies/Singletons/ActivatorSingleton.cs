using System;

namespace Velo.Dependencies.Singletons
{
    internal sealed class ActivatorSingleton : Dependency
    {
        private readonly Type _implementation;
        private readonly bool _isDisposable;
        
        private object _instance;

        public ActivatorSingleton(Type[] contracts, Type implementation) : base(contracts)
        {
            _implementation = implementation;
            _isDisposable = _implementation.IsAssignableFrom(typeof(IDisposable));
        }

        public override void Destroy()
        {
            if (_isDisposable)
            {
                ((IDisposable) _instance)?.Dispose();
            }
        }

        public override object Resolve(Type requestedType, DependencyContainer container)
        {
            return _instance ?? (_instance = container.Activate(_implementation));
        }
    }
}
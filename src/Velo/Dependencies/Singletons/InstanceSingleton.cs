using System;

namespace Velo.Dependencies.Singletons
{
    internal sealed class InstanceSingleton : Dependency
    {
        private object _instance;
        private readonly bool _isDisposable;

        public InstanceSingleton(Type[] contracts, object instance) : base(contracts)
        {
            _instance = instance;
            _isDisposable = instance.GetType().IsAssignableFrom(typeof(IDisposable));
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
            return _instance;
        }
    }
}
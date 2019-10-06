using System;
using Velo.Utils;

namespace Velo.Dependencies.Singletons
{
    internal sealed class InstanceSingleton : Dependency
    {
        private object _instance;
        private readonly bool _isDisposable;

        public InstanceSingleton(object instance) 
            : this(new []{ instance.GetType()}, instance)
        {
        }
        
        public InstanceSingleton(Type[] contracts, object instance) : base(contracts)
        {
            _instance = instance;
            _isDisposable = ReflectionUtils.IsDisposableType(instance.GetType());
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
            return _instance;
        }
    }
}
using System;
using System.Reflection;
using Velo.Utils;

namespace Velo.Dependencies.Singletons
{
    internal sealed class CompiledSingleton : Dependency
    {
        private Func<object> _builder;
        private readonly ConstructorInfo _constructor;
        private readonly bool _isDisposable;

        private object _instance;

        public CompiledSingleton(Type[] contracts, Type implementation) : base(contracts)
        {
            _constructor = ReflectionUtils.GetConstructor(implementation);
            _isDisposable = ReflectionUtils.IsDisposableType(implementation);
        }

        public override void Init(DependencyContainer container)
        {
            if (_builder == null)
            {
                _builder = container.CreateActivator<object>(_constructor);
            }
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

            _instance = _builder();
            return _instance;
        }
    }
}
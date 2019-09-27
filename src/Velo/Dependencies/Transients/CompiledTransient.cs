using System;
using System.Reflection;
using Velo.Utils;

namespace Velo.Dependencies.Transients
{
    internal sealed class CompiledTransient : Dependency
    {
        private Func<DependencyContainer, object> _builder;
        private readonly ConstructorInfo _constructor;

        public CompiledTransient(Type[] contracts, Type implementation) : base(contracts)
        {
            _constructor = ReflectionUtils.GetConstructor(implementation);
        }

        public override object Resolve(Type contract, DependencyContainer container)
        {
            if (_builder == null) _builder = container.CreateActivator<object>(_constructor);
            return _builder(container);
        }
    }
}
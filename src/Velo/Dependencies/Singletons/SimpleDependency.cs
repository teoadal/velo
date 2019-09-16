using System;
using System.Reflection;

using Velo.Utils;

namespace Velo.Dependencies.Singletons
{
    internal sealed class SimpleDependency : IDependency
    {
        private readonly ConstructorInfo _constructor;
        private readonly Type _contract;
        private readonly Type _implementation;
        private readonly bool _isDisposable;

        private object _instance;

        public SimpleDependency(Type contract, Type implementation)
        {
            _contract = contract;
            _implementation = implementation;

            _constructor = ReflectionUtils.GetConstructor(implementation);
            _isDisposable = ReflectionUtils.IsDisposableType(implementation);
        }

        public bool Applicable(Type contract)
        {
            return _contract == contract;
        }

        public void Destroy()
        {
            if (_isDisposable)
            {
                ((IDisposable) _instance)?.Dispose();
            }

            _instance = null;
        }

        public object Resolve(Type contract, DependencyContainer container)
        {
            if (_instance != null) return _instance;

            var parameters = _constructor.GetParameters();
            
            var resolvedParameters = new object[parameters.Length];
            for (var i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
        
                var parameterType = parameter.ParameterType;
                var parameterName = parameter.Name;
                var required = !parameter.HasDefaultValue;

                resolvedParameters[i] = container.Resolve(parameterType, parameterName, required);
            }
            
            _instance = _constructor.Invoke(resolvedParameters);
            return _instance;
        }
        
        public override string ToString()
        {
            return $"Dependency for {_contract.Name}";
        }
    }
}
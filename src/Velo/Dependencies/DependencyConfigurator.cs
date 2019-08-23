using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using Velo.Utils;

namespace Velo.Dependencies
{
    public sealed class DependencyConfigurator
    {
        private Func<DependencyContainer, object> _builder;
        private readonly List<Type> _contracts;
        private Type _implementation;
        private object _instance;
        private bool _scope;
        private bool _singleton;

        public DependencyConfigurator()
        {
            _contracts = new List<Type>(2);
        }

        public DependencyConfigurator Contract<T>()
        {
            return Contract(Typeof<T>.Raw);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DependencyConfigurator Contract(Type contract)
        {
            if (_implementation != null && !_implementation.IsAssignableFrom(contract))
            {
                throw new InvalidOperationException($"{contract} is not assignable from {_implementation}");
            }
            
            _contracts.Add(contract);
            return this;
        }

        public DependencyConfigurator Builder(Func<DependencyContainer, object> builder)
        {
            if (_implementation != null) throw InconsistentConfiguration();
            
            _builder = builder;
            return this;
        }

        public DependencyConfigurator Implementation<T>() 
        {
            return Implementation(typeof(T));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DependencyConfigurator Implementation(Type implementation)
        {
            if (_implementation != null || _builder != null || _instance != null)
            {
                throw InconsistentConfiguration();
            }

            if (_contracts.Count > 0)
            {
                foreach (var contract in _contracts)
                {
                    if (contract.IsAssignableFrom(implementation)) continue;
                    throw new InvalidOperationException($"{contract} is not assignable from {implementation}");
                }
            }

            _implementation = implementation;
            return this;
        }

        public DependencyConfigurator Instance(object instance)
        {
            if (_implementation != null || _builder != null || _instance != null)
            {
                throw InconsistentConfiguration();
            }
            
            _instance = instance;
            return this;
        }
        
        public DependencyConfigurator Scope(bool value = true)
        {
            _singleton = value;
            _scope = value;

            return this;
        }

        public DependencyConfigurator Singleton(bool value = true)
        {
            _singleton = value;
            return this;
        }

        internal IDependency Build()
        {
            var contracts = _contracts.ToArray();
            _contracts.Clear();
            
            throw new NotImplementedException();
        }

        private Exception InconsistentConfiguration()
        {
            return new InvalidOperationException("Inconsistent dependency configuration");
        }
    }
}
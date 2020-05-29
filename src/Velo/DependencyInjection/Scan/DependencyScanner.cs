using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Velo.Utils;

namespace Velo.DependencyInjection.Scan
{
    public sealed class DependencyScanner
    {
        private readonly HashSet<Assembly> _assemblies;
        private readonly List<IDependencyCollector> _collectors;
        private readonly DependencyCollection _dependencyCollection;

        internal DependencyScanner(DependencyCollection dependencyCollection)
        {
            _assemblies = new HashSet<Assembly>();
            _collectors = new List<IDependencyCollector>();
            _dependencyCollection = dependencyCollection;
        }

        public DependencyScanner Assembly(Assembly assembly)
        {
            _assemblies.Add(assembly);

            return this;
        }

        public DependencyScanner AssemblyOf<T>()
        {
            _assemblies.Add(typeof(T).Assembly);

            return this;
        }

        public DependencyScanner Register(Type contract, DependencyLifetime lifetime)
        {
            if (contract.IsGenericTypeDefinition)
            {
                _collectors.Add(new GenericInterfaceCollector(contract, lifetime));
            }
            else
            {
                _collectors.Add(new AssignableCollector(contract, lifetime));
            }

            return this;
        }
        
        public DependencyScanner UseCollector(IDependencyCollector collector)
        {
            _collectors.Add(collector);

            return this;
        }

        public void Execute()
        {
            if (_assemblies.Count == 0)
            {
                throw Error.InvalidOperation("Set assemblies for scan dependencies");
            }
            
            var anonType = typeof(CompilerGeneratedAttribute);
            foreach (var assembly in _assemblies)
            {
                foreach (var definedType in assembly.DefinedTypes)
                {
                    if (definedType.IsAbstract || Attribute.IsDefined(definedType, anonType) || definedType.IsInterface)
                    {
                        continue;
                    }

                    foreach (var collector in _collectors)
                    {
                        collector.TryRegister(_dependencyCollection, definedType);
                    }
                }
            }
        }
    }
}
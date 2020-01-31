using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Velo.Utils;

namespace Velo.DependencyInjection.Scan
{
    public sealed class DependencyScanner
    {
        public DependencyCollection Collection => _dependencyCollection;
        
        private readonly HashSet<Assembly> _assemblies;
        private readonly List<IDependencyAllover> _alloverCollection;
        private readonly DependencyCollection _dependencyCollection;

        internal DependencyScanner(DependencyCollection dependencyCollection)
        {
            _assemblies = new HashSet<Assembly>();
            _alloverCollection = new List<IDependencyAllover>();
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

        #region ScopedOf

        public DependencyScanner ScopedOf(Type contract)
        {
            if (contract.IsGenericTypeDefinition)
            {
                _alloverCollection.Add(new GenericInterfaceAllover(contract, DependencyLifetime.Scope));
            }
            else
            {
                _alloverCollection.Add(new AssignableAllover(contract, DependencyLifetime.Scope));
            }

            return this;
        }

        public DependencyScanner ScopedOf<TContract>()
            where TContract : class
        {
            return ScopedOf(Typeof<TContract>.Raw);
        }

        #endregion

        #region SingletonOf

        public DependencyScanner SingletonOf<TContract>()
            where TContract: class
        {
            return SingletonOf(Typeof<TContract>.Raw);
        }

        public DependencyScanner SingletonOf(Type contract)
        {
            if (contract.IsGenericTypeDefinition)
            {
                _alloverCollection.Add(new GenericInterfaceAllover(contract, DependencyLifetime.Singleton));
            }
            else
            {
                _alloverCollection.Add(new AssignableAllover(contract, DependencyLifetime.Singleton));
            }

            return this;
        }

        #endregion

        #region TransientOf

        public DependencyScanner TransientOf<TContract>()
            where TContract: class
        {
            return TransientOf(Typeof<TContract>.Raw);
        }

        public DependencyScanner TransientOf(Type contract)
        {
            if (contract.IsGenericTypeDefinition)
            {
                _alloverCollection.Add(new GenericInterfaceAllover(contract, DependencyLifetime.Transient));
            }
            else
            {
                _alloverCollection.Add(new AssignableAllover(contract, DependencyLifetime.Transient));
            }

            return this;
        }

        #endregion

        public DependencyScanner UseAllover(IDependencyAllover allover)
        {
            _alloverCollection.Add(allover);

            return this;
        }

        internal void Execute()
        {
            var anonType = typeof(CompilerGeneratedAttribute);
            foreach (var assembly in _assemblies)
            {
                foreach (var definedType in assembly.DefinedTypes)
                {
                    if (definedType.IsAbstract || Attribute.IsDefined(definedType, anonType) || definedType.IsInterface)
                    {
                        continue;
                    }

                    foreach (var allover in _alloverCollection)
                    {
                        allover.TryRegister(_dependencyCollection, definedType);
                    }
                }
            }
        }
    }
}
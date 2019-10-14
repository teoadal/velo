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
        private readonly List<DependencyAllover> _alloverCollection;

        internal DependencyScanner()
        {
            _assemblies = new HashSet<Assembly>();
            _alloverCollection = new List<DependencyAllover>();
        }

        public DependencyScanner Assembly(Assembly assembly)
        {
            _assemblies.Add(assembly);
            
            return this;
        }

        public DependencyScanner ScopedOf<TContract>()
        {
            _alloverCollection.Add(new AssignableAllover(Typeof<TContract>.Raw, DependencyLifetime.Scope));
            
            return this;
        }
        
        public DependencyScanner ScopedOfGenericInterface(Type genericInterface)
        {
            _alloverCollection.Add(new GenericInterfaceAllover(genericInterface, DependencyLifetime.Scope));
            
            return this;
        }
        
        public DependencyScanner SingletonOf<TContract>()
        {
            _alloverCollection.Add(new AssignableAllover(Typeof<TContract>.Raw, DependencyLifetime.Singleton));
            
            return this;
        }
        
        public DependencyScanner SingletonOfGenericInterface(Type genericInterface)
        {
            _alloverCollection.Add(new GenericInterfaceAllover(genericInterface, DependencyLifetime.Singleton));
            
            return this;
        }
        
        public DependencyScanner TransientOf<TContract>()
        {
            _alloverCollection.Add(new AssignableAllover(Typeof<TContract>.Raw, DependencyLifetime.Transient));
            
            return this;
        }
        
        public DependencyScanner TransientOfGenericInterface(Type genericInterface)
        {
            _alloverCollection.Add(new GenericInterfaceAllover(genericInterface, DependencyLifetime.Singleton));
            
            return this;
        }
        
        public DependencyScanner UseAllover(DependencyAllover allover)
        {
            _alloverCollection.Add(allover);
            
            return this;
        }
        
        internal void Execute(DependencyCollection dependencyCollection)
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
                        allover.TryRegister(dependencyCollection, definedType);
                    }
                }
            }
        }
    }
}
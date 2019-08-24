using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Velo.Utils;

namespace Velo.Dependencies.Scan
{
    public sealed class AssemblyScanner
    {
        private readonly DependencyBuilder _dependencyBuilder;

        private readonly List<Assembly> _assemblies;
        private readonly List<IDependencyScanner> _scanners;

        internal AssemblyScanner(DependencyBuilder builder)
        {
            _dependencyBuilder = builder;

            _assemblies = new List<Assembly>();
            _scanners = new List<IDependencyScanner>();
        }

        public AssemblyScanner Assembly(Assembly assembly)
        {
            _assemblies.Add(assembly);
            return this;
        }

        public AssemblyScanner Assemblies(IEnumerable<Assembly> assembly)
        {
            _assemblies.AddRange(_assemblies);
            return this;
        }
        
        public AssemblyScanner RegisterAsSingleton<TContract>()
        {
            var contract = Typeof<TContract>.Raw;
            var scanner = new ParentScanner(contract);
            
            return UseScanner(scanner);
        }

        public AssemblyScanner UseScanner(IDependencyScanner scanner)
        {
            _scanners.Add(scanner);
            return this;
        }

        internal void Scan()
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

                    foreach (var scanner in _scanners)
                    {
                        if (scanner.Applicable(definedType))
                        {
                            scanner.Register(_dependencyBuilder, definedType);
                        }
                    }
                }
            }
        }
    }
}
using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Velo.Extensions.DependencyInjection.Scan
{
    internal abstract class AssemblyScanner
    {
        public void Scan(params Assembly[] assemblies)
        {
            var anonType = typeof(CompilerGeneratedAttribute);

            foreach (var assembly in assemblies)
            {
                foreach (var definedType in assembly.DefinedTypes)
                {
                    if (definedType.IsAbstract || Attribute.IsDefined(definedType, anonType) || definedType.IsInterface)
                    {
                        continue;
                    }

                    Handle(definedType);
                }
            }
        }

        protected abstract void Handle(Type implementation);
    }
}
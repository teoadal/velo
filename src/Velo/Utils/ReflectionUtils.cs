using System;
using System.Reflection;

namespace Velo.Utils
{
    internal static class ReflectionUtils
    {
        public static ConstructorInfo GetConstructor(Type type)
        {
            if (type.IsAbstract) throw new InvalidOperationException("Type is abstract");
            if (type.IsInterface) throw new InvalidOperationException("Type is interface");
            
            var availableConstructors = type.GetConstructors();
            return availableConstructors[0];
        }
    }
}
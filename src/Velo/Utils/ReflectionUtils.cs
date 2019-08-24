using System;
using System.Reflection;

namespace Velo.Utils
{
    internal static class ReflectionUtils
    {
        public static ConstructorInfo GetConstructor(Type type)
        {
            return type.GetConstructors()[0];
        }
    }
}
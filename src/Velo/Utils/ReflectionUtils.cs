using System;
using System.Reflection;

namespace Velo.Utils
{
    internal static class ReflectionUtils
    {
        private static readonly Type DisposableInterfaceType = typeof(IDisposable);

        public static ConstructorInfo GetConstructor(Type type)
        {
            if (type.IsAbstract) throw Error.InvalidOperation("Type is abstract");
            if (type.IsInterface) throw Error.InvalidOperation("Type is interface");

            var availableConstructors = type.GetConstructors();
            return availableConstructors[0];
        }

        public static Type[] GetGenericInterfaceParameters(Type type, Type genericInterface)
        {
            var typeInterfaces = type.GetInterfaces();

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < typeInterfaces.Length; i++)
            {
                var typeInterface = typeInterfaces[i];
                if (typeInterface.IsGenericType && typeInterface.GetGenericTypeDefinition() == genericInterface)
                {
                    return typeInterface.GenericTypeArguments;
                }
            }

            throw Error.NotFound($"Generic interface ${genericInterface.Name} is not implemented");
        }

        public static bool IsDisposableType(Type type)
        {
            return DisposableInterfaceType.IsAssignableFrom(type);
        }
    }
}
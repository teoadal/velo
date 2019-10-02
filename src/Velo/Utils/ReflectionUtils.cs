using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Velo.Utils
{
    internal static class ReflectionUtils
    {
        private static readonly Type DisposableInterfaceType = typeof(IDisposable);

        public static Array CreateArray<T>(Type elementType, List<T> list)
        {
            var array = Array.CreateInstance(elementType, list.Count);
            for (var i = 0; i < list.Count; i++)
            {
                array.SetValue(list[i], i);
            }

            return array;
        }

        public static ConstructorInfo GetConstructor(Type type)
        {
            if (type.IsAbstract) throw Error.InvalidOperation("Type is abstract");
            if (type.IsInterface) throw Error.InvalidOperation("Type is interface");

            var availableConstructors = type.GetTypeInfo().DeclaredConstructors;
            return availableConstructors.FirstOrDefault(constructor => !constructor.IsStatic);
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

        public static bool TryGetAttribute<TAttribute>(Type type, out TAttribute attribute)
            where TAttribute : Attribute
        {
            attribute = type.GetCustomAttribute<TAttribute>();
            return attribute != null;
        }
    }
}
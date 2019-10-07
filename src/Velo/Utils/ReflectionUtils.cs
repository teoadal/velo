using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Velo.Utils
{
    internal static class ReflectionUtils
    {
        private static readonly Type DisposableInterfaceType = typeof(IDisposable);

        public static ConstructorInfo GetConstructor(Type type)
        {
            if (type.IsAbstract) throw Error.InvalidOperation("Type is abstract or static");
            if (type.IsInterface) throw Error.InvalidOperation("Type is interface");

            var availableConstructors = type.GetTypeInfo().DeclaredConstructors;
            return availableConstructors.FirstOrDefault(constructor => !constructor.IsStatic);
        }

        public static ConstructorInfo GetEmptyConstructor(Type type)
        {
            if (type.IsAbstract) throw Error.InvalidOperation("Type is abstract or static");
            if (type.IsInterface) throw Error.InvalidOperation("Type is interface");

            var availableConstructors = type.GetTypeInfo().DeclaredConstructors;
            return availableConstructors.FirstOrDefault(c => !c.IsStatic && c.GetParameters().Length == 0);
        }

        public static bool HasEmptyConstructor(Type type)
        {
            var availableConstructors = type.GetTypeInfo().DeclaredConstructors;
            return availableConstructors.Any(constructor => constructor.GetParameters().Length == 0);
        }

        public static Type[] GetGenericInterfaceParameters(Type type, Type genericInterface)
        {
            if (!genericInterface.IsInterface)
            {
                throw Error.InvalidOperation($"Is not interface");
            }
            
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

            throw Error.NotFound($"Generic interface {genericInterface.Name} is not implemented");
        }

        public static Type[] GetGenericInterfaces(Type type, Type genericInterface)
        {
            if (!genericInterface.IsInterface)
            {
                throw Error.InvalidOperation($"Is not interface");
            }

            var implementations = new List<Type>();
            var typeInterfaces = type.GetInterfaces();
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < typeInterfaces.Length; i++)
            {
                var typeInterface = typeInterfaces[i];
                if (typeInterface.IsGenericType && typeInterface.GetGenericTypeDefinition() == genericInterface)
                {
                    implementations.Add(typeInterface);
                }
            }

            if (implementations.Count == 0)
            {
                throw Error.NotFound($"Generic interface {genericInterface.Name} is not implemented");    
            }

            return implementations.ToArray();
        }
        
        public static bool IsDisposableType(Type type)
        {
            return DisposableInterfaceType.IsAssignableFrom(type);
        }

        public static bool IsGenericInterfaceImplementation(Type type, Type genericInterface)
        {
            var typeInterfaces = type.GetInterfaces();

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < typeInterfaces.Length; i++)
            {
                var typeInterface = typeInterfaces[i];
                if (typeInterface.IsGenericType && typeInterface.GetGenericTypeDefinition() == genericInterface)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool TryGetAttribute<TAttribute>(Type type, out TAttribute attribute)
            where TAttribute : Attribute
        {
            attribute = type.GetCustomAttribute<TAttribute>();
            return attribute != null;
        }
    }
}
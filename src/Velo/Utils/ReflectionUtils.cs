using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Velo.Collections;

namespace Velo.Utils
{
    internal static class ReflectionUtils
    {
        private static readonly Type DisposableInterfaceType = typeof(IDisposable);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Type GetArrayElementType(Type arrayType)
        {
            var elementType = arrayType.GetElementType();
            if (elementType == null) throw Error.InvalidData($"Type '{GetName(arrayType)}' is not array");
            return elementType;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ConstructorInfo GetConstructor(Type type)
        {
            CheckIsNotAbstractAndNotInterface(type);

            var availableConstructors = type.GetTypeInfo().DeclaredConstructors;
            return availableConstructors.FirstOrDefault(constructor => !constructor.IsStatic);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ConstructorInfo GetEmptyConstructor(Type type)
        {
            CheckIsNotAbstractAndNotInterface(type);

            var availableConstructors = type.GetTypeInfo().DeclaredConstructors;
            return availableConstructors.FirstOrDefault(c => !c.IsStatic && c.GetParameters().Length == 0);
        }

        public static string GetName(Type type, StringBuilder sb = null)
        {
            var builder = sb ?? new StringBuilder();

            if (type.IsArray)
            {
                GetName(GetArrayElementType(type), builder);
                builder.Append("[]");
            }
            else if (type.IsGenericType)
            {
                var genericDefinitionName = type.GetGenericTypeDefinition().Name;
                builder
                    .Append(genericDefinitionName.Remove(genericDefinitionName.IndexOf('`')))
                    .Append('<');

                var genericArguments = type.GenericTypeArguments;
                for (var i = 0; i < genericArguments.Length; i++)
                {
                    if (i > 0) builder.Append(", ");
                    GetName(genericArguments[i], builder);
                }

                builder.Append('>');
            }
            else builder.Append(type.Name);

            return sb == null ? builder.ToString() : null;
        }

        public static Type[] GetGenericInterfaceParameters(Type type, Type genericInterface)
        {
            CheckIsGenericInterfaceTypeDefinition(genericInterface);

            if (type.IsInterface && IsGenericTypeImplementation(type, genericInterface))
            {
                return type.GenericTypeArguments;
            }

            var interfaces = type.GetInterfaces();
            for (var i = 0; i < interfaces.Length; i++)
            {
                var typeInterface = interfaces[i];
                if (IsGenericTypeImplementation(typeInterface, genericInterface))
                {
                    return typeInterface.GenericTypeArguments;
                }
            }

            throw Error.NotFound($"Generic interface {GetName(genericInterface)} is not implemented");
        }

        public static LocalVector<Type> GetGenericInterfaceImplementations(Type type, Type genericInterface)
        {
            CheckIsGenericInterfaceTypeDefinition(genericInterface);

            var implementations = new LocalVector<Type>();

            if (type.IsInterface && IsGenericTypeImplementation(type, genericInterface))
            {
                implementations.Add(type);
            }

            var interfaces = type.GetInterfaces();
            for (var i = 0; i < interfaces.Length; i++)
            {
                var typeInterface = interfaces[i];
                if (IsGenericTypeImplementation(typeInterface, genericInterface))
                {
                    implementations.Add(typeInterface);
                }
            }

            if (implementations.Length == 0)
            {
                throw Error.NotFound($"Generic interface {GetName(genericInterface)} is not implemented");
            }

            return implementations;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDisposable<T>(T instance, out IDisposable disposable)
        {
            disposable = instance as IDisposable;
            return disposable != null;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDisposableType(Type type)
        {
            return DisposableInterfaceType.IsAssignableFrom(type);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsGenericTypeImplementation(Type type, Type genericType)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == genericType;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasEmptyConstructor(Type type)
        {
            var availableConstructors = type.GetTypeInfo().DeclaredConstructors;
            return availableConstructors.Any(constructor => constructor.GetParameters().Length == 0);
        }

        public static bool TryGetAttribute<TAttribute>(Type type, out TAttribute attribute)
            where TAttribute : Attribute
        {
            attribute = type.GetCustomAttribute<TAttribute>();
            return attribute != null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void CheckIsGenericInterfaceTypeDefinition(Type type)
        {
            if (!type.IsInterface || !type.IsGenericTypeDefinition)
            {
                throw Error.InvalidData($"'{GetName(type)}' is not generic interface definition");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void CheckIsNotAbstractAndNotInterface(Type type)
        {
            if (type.IsAbstract) throw Error.InvalidOperation($"'{GetName(type)}' is abstract or static");
            if (type.IsInterface) throw Error.InvalidOperation($"'{GetName(type)}' is interface");
        }
    }
}
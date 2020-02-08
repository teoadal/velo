using System;
using System.IO;
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

        public static LocalList<Type> GetInterfaceImplementations(Type type, Type interfaceType)
        {
            var implementations = new LocalList<Type>();

            var typeInterfaces = type.GetInterfaces();
            foreach (var typeInterface in typeInterfaces)
            {
                if (typeInterface.IsAssignableFrom(interfaceType))
                {
                    implementations.Add(typeInterface);
                }
            }
            
            return implementations;
        }
        
        public static string GetName<T>()
        {
            return GetName(typeof(T));
        }
        
        public static string GetName(Type type, StringBuilder sb = null)
        {
            var builder = sb ?? new StringBuilder();
            WriteName(type, new StringWriter(builder));
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
            foreach (var typeInterface in interfaces)
            {
                if (IsGenericTypeImplementation(typeInterface, genericInterface))
                {
                    return typeInterface.GenericTypeArguments;
                }
            }

            throw Error.NotFound($"Generic interface {GetName(genericInterface)} is not implemented");
        }

        public static LocalList<Type> GetGenericInterfaceImplementations(Type type, Type genericInterface, bool throwIfNotFound = true)
        {
            CheckIsGenericInterfaceTypeDefinition(genericInterface);

            var implementations = new LocalList<Type>();

            if (type.IsInterface && IsGenericTypeImplementation(type, genericInterface))
            {
                implementations.Add(type);
            }

            var interfaces = type.GetInterfaces();
            foreach (var typeInterface in interfaces)
            {
                if (IsGenericTypeImplementation(typeInterface, genericInterface))
                {
                    implementations.Add(typeInterface);
                }
            }

            if (throwIfNotFound && implementations.Length == 0)
            {
                throw Error.NotFound($"Generic interface {GetName(genericInterface)} is not implemented");
            }

            return implementations;
        }

        public static LocalList<Type> GetGenericInterfaceImplementations(Type type, params Type[] genericInterfaces)
        {
            var implementations = new LocalList<Type>();
            foreach (var genericInterface in genericInterfaces)
            {
                CheckIsGenericInterfaceTypeDefinition(genericInterface);
                var currentImplementations = GetGenericInterfaceImplementations(type, genericInterface, false);
                implementations.AddRange(currentImplementations);
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
        
        public static void WriteName(Type type, TextWriter writer)
        {
            if (type.IsArray)
            {
                WriteName(GetArrayElementType(type), writer);
                writer.Write("[]");
            }
            else if (type.IsGenericType)
            {
                var genericDefinitionName = type.GetGenericTypeDefinition().Name;
                writer.Write(genericDefinitionName.Remove(genericDefinitionName.IndexOf('`')));
                writer.Write('<');
                
                var genericArguments = type.GenericTypeArguments;
                for (var i = 0; i < genericArguments.Length; i++)
                {
                    if (i > 0) writer.Write(", ");
                    WriteName(genericArguments[i], writer);
                }

                writer.Write('>');
            }
            else writer.Write(type.Name);
        }
    }
}
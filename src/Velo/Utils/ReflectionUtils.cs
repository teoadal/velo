using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Velo.Collections.Local;

namespace Velo.Utils
{
    internal static class ReflectionUtils
    {
        private static readonly Type DisposableInterfaceType = typeof(IDisposable);

        private static readonly Type[] ArrayLikeGenericTypes =
        {
            typeof(ICollection<>),
            typeof(IEnumerable<>),
            typeof(IReadOnlyCollection<>),
        };

        private static readonly Type[] ListGenericTypes =
        {
            typeof(List<>),
            typeof(IList<>)
        };


        public static TDelegate BuildStaticMethodDelegate<TDelegate>(MethodInfo methodInfo)
            where TDelegate : Delegate
        {
            var methodDelegate = methodInfo.CreateDelegate(typeof(TDelegate), null);
            return (TDelegate) methodDelegate;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Type GetArrayElementType(Type arrayType)
        {
            var elementType = arrayType.GetElementType();
            if (elementType == null) throw Error.InvalidData($"Type '{GetName(arrayType)}' is not array");
            return elementType;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Type GetCollectionElementType(Type collectionType)
        {
            return collectionType.IsArray
                ? GetArrayElementType(collectionType)
                : collectionType.GenericTypeArguments[0];
        }

        public static Assembly[] GetUserAssemblies()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            var result = new LocalList<Assembly>();
            foreach (var assembly in assemblies)
            {
                if (assembly.IsDynamic) continue;

                var name = assembly.FullName;
                if (name.StartsWith("Microsoft") || name.StartsWith("System") || name.StartsWith("NuGet") ||
                    name.StartsWith("netstandard") || name.StartsWith("mscorlib") || name.StartsWith("xunit") ||
                    name.StartsWith("Newtonsoft") || name.StartsWith("JetBrains") || name.StartsWith("Castle") ||
                    name.StartsWith("Moq") || name.StartsWith("AutoFixture") || name.StartsWith("FluentAssertions") ||
                    name == "Velo")
                {
                    continue;
                }

                result.Add(assembly);
            }

            return result.ToArray();
        }

        /// <summary>
        /// Get first declared not static constructor
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ConstructorInfo? GetConstructor(Type type)
        {
            CheckIsNotAbstractAndNotInterface(type);

            var availableConstructors = type.GetTypeInfo().DeclaredConstructors;
            return availableConstructors.FirstOrDefault(constructor => !constructor.IsStatic);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ConstructorInfo? GetEmptyConstructor(Type type)
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
            var builder = new StringBuilder();
            WriteName(typeof(T), new StringWriter(builder));

            return builder.ToString();
        }

        public static string GetName(Type type)
        {
            var builder = new StringBuilder();
            WriteName(type, new StringWriter(builder));

            return builder.ToString();
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

        public static LocalList<Type> GetGenericInterfaceImplementations(Type type, Type genericInterface,
            bool throwIfNotFound = true)
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

        public static bool IsArrayLikeGenericType(Type type, out Type elementType)
        {
            if (type.IsArray)
            {
                elementType = GetArrayElementType(type);
                return true;
            }

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var generic in ArrayLikeGenericTypes)
            {
                if (!IsGenericTypeImplementation(type, generic)) continue;

                elementType = type.GenericTypeArguments[0];
                return true;
            }

            elementType = null!;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDisposable<T>(T instance, out IDisposable disposable)
        {
            if (instance is IDisposable result)
            {
                disposable = result;
                return true;
            }

            disposable = null!;
            return false;
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
        public static bool IsGenericInterfaceImplementation(Type type, Type genericInterface)
        {
            var interfaces = type.GetInterfaces();

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var @interface in interfaces)
            {
                if (@interface.IsGenericType && @interface.GetGenericTypeDefinition() == genericInterface)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsListLikeGenericType(Type type, out Type elementType)
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var generic in ListGenericTypes)
            {
                if (!IsGenericTypeImplementation(type, generic)) continue;

                elementType = type.GenericTypeArguments[0];
                return true;
            }

            elementType = null!;
            return false;
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

        public static object TryInvoke(ConstructorInfo constructor, object?[]? parameters = null)
        {
            try
            {
                return constructor.Invoke(parameters ?? Array.Empty<object>());
            }
            catch (TargetInvocationException e)
            {
                Exception exception = e;
                while (exception.InnerException != null)
                {
                    exception = exception.InnerException;
                }

                throw exception;
            }
        }

        public static void WriteName(Type type, StringBuilder sb)
        {
            var writer = new StringWriter(sb);
            WriteName(type, writer);
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
            if (type.IsInterface)
            {
                throw Error.InvalidOperation($"Type '{GetName(type)}' is interface");
            }

            if (type.IsAbstract)
            {
                throw Error.InvalidOperation($"Type '{GetName(type)}' is abstract or static");
            }
        }
    }
}
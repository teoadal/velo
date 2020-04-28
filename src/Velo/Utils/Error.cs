using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using Velo.Serialization.Tokenization;

namespace Velo.Utils
{
    internal static class Error
    {
        public static ArgumentException AlreadyExists(string? message = null)
        {
            return new ArgumentException(message ?? "Element already exists");
        }
        
        public static TypeAccessException CircularDependency(Type contract)
        {
            return new TypeAccessException($"Detected circular dependency '{ReflectionUtils.GetName(contract)}'");
        }

        public static InvalidCastException Cast(string message, Exception? innerException = null)
        {
            return new InvalidCastException(message, innerException);
        }

        public static KeyNotFoundException DependencyNotRegistered(Type contract)
        {
            var name = ReflectionUtils.GetName(contract);
            return new KeyNotFoundException($"Dependency with contract '{name}' is not registered");
        }
        
        public static KeyNotFoundException DefaultConstructorNotFound(Type type)
        {
            return new KeyNotFoundException($"Default constructor for '{ReflectionUtils.GetName(type)}' not found");
        }

        public static SerializationException Deserialization(string message)
        {
            return new SerializationException(message);
        }
        
        public static SerializationException Deserialization(JsonTokenType expected, JsonTokenType actual)
        {
            return new SerializationException($"Expected {expected} json token, but found {actual}");
        }
        
        public static ObjectDisposedException Disposed(string objectName)
        {
            return new ObjectDisposedException(objectName);
        }

        public static InvalidOperationException EnumerableChanged()
        {
            return new InvalidOperationException("Enumerable was changed");
        }
        
        public static InvalidOperationException InconsistentOperation(string message)
        {
            return new InvalidOperationException($"Inconsistent operation: {message}");
        }

        public static InvalidDataException InvalidData(string message)
        {
            return new InvalidDataException(message);
        }

        public static InvalidDataException InvalidDependencyLifetime(string? message = null)
        {
            return new InvalidDataException(message ?? "Invalid dependency lifetime");
        }
        
        public static InvalidOperationException InvalidOperation(string message)
        {
            return new InvalidOperationException(message);
        }

        public static FileNotFoundException FileNotFound(string path)
        {
            return new FileNotFoundException($"Required file '{path}' not found", Path.GetFileName(path));
        }
        
        public static IndexOutOfRangeException OutOfRange(string? message = null)
        {
            return new IndexOutOfRangeException(message ?? "Index out of range");
        }
        
        public static KeyNotFoundException NotFound(string? message = null)
        {
            return new KeyNotFoundException(message ?? "Element not found");
        }

        public static AmbiguousMatchException NotSingle(string message)
        {
            return new AmbiguousMatchException(message);
        }

        public static ArgumentNullException Null(string argumentName)
        {
            return new ArgumentNullException(argumentName);
        }
    }
}
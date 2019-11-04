using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Velo.Utils
{
    internal static class Error
    {
        public static TypeAccessException CircularDependency(Type contract)
        {
            return new TypeAccessException($"Detected circular dependency '{ReflectionUtils.GetName(contract)}'");
        }

        public static KeyNotFoundException DefaultConstructorNotFound(Type type)
        {
            return new KeyNotFoundException($"Default constructor for '{ReflectionUtils.GetName(type)}' not found");
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

        public static InvalidOperationException InvalidOperation(string message)
        {
            return new InvalidOperationException(message);
        }

        public static IndexOutOfRangeException OutOfRange(string message = null)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                message = "Index out of range";
            }
            
            return new IndexOutOfRangeException(message);
        }
        
        public static KeyNotFoundException NotFound(string message = null)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                message = "Element not found";
            }
            
            return new KeyNotFoundException(message);
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
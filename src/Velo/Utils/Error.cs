using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using Velo.Dependencies;

namespace Velo.Utils
{
    internal static class Error
    {
        public static SerializationException BadConverter(Type type)
        {
            return new SerializationException($"Bad converter for type {type}");
        }

        public static TypeAccessException CircularDependency(IDependency dependency)
        {
            return new TypeAccessException($"Detected circular dependency {dependency}");
        }

        public static ObjectDisposedException Disposed(string objectName)
        {
            return new ObjectDisposedException(objectName);
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

        public static KeyNotFoundException NotFound(string message)
        {
            return new KeyNotFoundException(message);
        }
    }
}
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Velo.Utils
{
    internal static class Typeof<T>
    {
        public static readonly int Id = Typeof.GetTypeId(typeof(T));

        public static readonly Type Raw = typeof(T);
    }

    internal static class Typeof
    {
        private static readonly ConcurrentDictionary<Type, int> RegisteredTypes;
        private static int _nextId;

        static Typeof()
        {
            RegisteredTypes = new ConcurrentDictionary<Type, int>(Environment.ProcessorCount, 100);
        }

        public static int GetTypeId(Type type)
        {
            return RegisteredTypes.GetOrAdd(type, _ => Interlocked.Increment(ref _nextId));
        }
    }
}
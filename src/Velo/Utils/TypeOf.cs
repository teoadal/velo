using System;
using System.Collections.Generic;

namespace Velo.Utils
{
    internal static class Typeof<T>
    {
        public static readonly int Id = Typeof.GetTypeId(typeof(T));
        public static readonly Type Raw = typeof(T);
    }

    internal static class Typeof
    {
        private static readonly Dictionary<Type, int> RegisteredTypes = new Dictionary<Type, int>();
        private static int _nextId;

        public static int GetTypeId(Type type)
        {
            using (Lock.Enter(RegisteredTypes))
            {
                if (RegisteredTypes.TryGetValue(type, out var existsId)) return existsId;

                var id = _nextId++;
                RegisteredTypes.Add(type, id);

                return id;
            }
        }
    }
}
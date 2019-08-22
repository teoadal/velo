using System;
using System.Collections.Generic;

namespace Velo.Utils
{
    internal static class Typeof<T>
    {
        public static readonly int Id = TypeOf.GetId(typeof(T));
        
        public static readonly Type Raw = typeof(T);
    }
    
    internal static class TypeOf
    {
        private static readonly Dictionary<Type, int> RegisteredTypes;
        private static int _nextid;

        static TypeOf()
        {
            RegisteredTypes = new Dictionary<Type, int>(100);
            _nextid = 0;
        }
        
        public static int GetId(Type type)
        {
            if (RegisteredTypes.TryGetValue(type, out var existsId)) return existsId;

            var typeId = _nextid++;
            
            RegisteredTypes.Add(type, typeId);
            
            return typeId;
        }
    }
}
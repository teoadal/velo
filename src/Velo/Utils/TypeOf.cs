using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Velo.Utils
{
    internal static class Typeof<T>
    {
        public static readonly Type Raw = typeof(T);
        
        // ReSharper disable once StaticMemberInGenericType
        public static readonly int Id = Typeof.GetTypeId(Raw);
    }

    internal static class Typeof
    {
        private static readonly Dictionary<Type, int> RegisteredTypes;
        private static int _nextId;

        private static SpinLock _lock;

        static Typeof()
        {
            RegisteredTypes = new Dictionary<Type, int>(100);
            _lock = new SpinLock(Debugger.IsAttached);
        }

        public static int GetTypeId(Type type)
        {
            var lockTaken = false;
            try
            {
                _lock.Enter(ref lockTaken);

                if (RegisteredTypes.TryGetValue(type, out var exists)) return exists;
                
                _nextId++;
                RegisteredTypes.Add(type, _nextId);
                return _nextId;
            }
            finally
            {
                if (lockTaken) _lock.Exit();
            }
        }
    }
}
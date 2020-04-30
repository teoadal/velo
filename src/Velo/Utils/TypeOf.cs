using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Velo.Collections;

namespace Velo.Utils
{
    internal static class Typeof<T>
    {
        // ReSharper disable once StaticMemberInGenericType
        public static readonly int Id = Typeof.GetNextTypeId();

        public static readonly Type Raw = typeof(T);
    }

    internal static class Typeof
    {
        private static int _nextId;

        private static readonly DangerousVector<Type, int> RegisteredTypes;

        static Typeof()
        {
            RegisteredTypes = new DangerousVector<Type, int>(100);
        }

        public static int GetTypeId(Type type)
        {
            return RegisteredTypes.GetOrAdd(type, key => (int) typeof(Typeof<>)
                .MakeGenericType(key)
                .GetField(nameof(Typeof<object>.Id))
                .GetValue(null));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int GetNextTypeId() => Interlocked.Increment(ref _nextId);
    }
}
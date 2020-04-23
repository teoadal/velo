using System;

namespace Velo.Patching.Methods
{
    public readonly struct CommonMethods<T, TValue>
    {
        public readonly Func<T, TValue> Getter;
        public readonly Action<T>? Initializer;
        public readonly Action<T, TValue> Setter;

        public CommonMethods(Action<T>? initializer, Func<T, TValue> getter, Action<T, TValue> setter)
        {
            Initializer = initializer;
            Getter = getter;
            Setter = setter;
        }
    }
}
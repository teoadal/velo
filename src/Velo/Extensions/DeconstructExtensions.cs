using System.Collections.Generic;

namespace Velo.Extensions
{
    public static class DeconstructExtensions
    {
        public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> pair, out TKey key,
            out TValue value)
        {
            key = pair.Key;
            value = pair.Value;
        }
    }
}
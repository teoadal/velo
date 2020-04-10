using System.Runtime.CompilerServices;

namespace Velo.Collections
{
    public static class CollectionExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool NullOrEmpty<T>(this T[] array)
        {
            return array == null || array.Length == 0;
        }
    }
}
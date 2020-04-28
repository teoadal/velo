using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace Velo.Utils
{
    public static class StringUtils
    {
        public static readonly StringComparer IgnoreCaseComparer = StringComparer.OrdinalIgnoreCase;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Release(StringBuilder stringBuilder)
        {
            var result = stringBuilder.ToString();
            stringBuilder.Clear();
            return result;
        }
    }
}
using System.Runtime.CompilerServices;
using System.Text;

namespace Velo.Utils
{
    public static class StringUtils
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Release(StringBuilder stringBuilder)
        {
            var result = stringBuilder.ToString();
            stringBuilder.Clear();
            return result;
        }
    }
}
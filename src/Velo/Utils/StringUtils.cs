using System.IO;
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
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Release(StringWriter stringWriter)
        {
            var sb = stringWriter.GetStringBuilder();
            var result = sb.ToString();

            sb.Clear();
            stringWriter.Dispose();
            
            return result;
        }
    }
}
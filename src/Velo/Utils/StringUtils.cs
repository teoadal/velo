using System.IO;

namespace Velo.Utils
{
    public static class StringUtils
    {
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
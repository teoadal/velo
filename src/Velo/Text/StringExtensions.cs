using System.Text;

namespace Velo.Text
{
    internal static class StringExtensions
    {
        public static string Cut(this string source, string oldValue)
        {
            return source.Replace(oldValue, string.Empty);
        }
        
        public static string Cut(this string source, string oldValue1, string oldValue2)
        {
            return new StringBuilder(source)
                .Replace(oldValue1, string.Empty)
                .Replace(oldValue2, string.Empty)
                .ToString();
        }
        
        public static string Cut(this string source, string oldValue1, string oldValue2, string oldValue3)
        {
            return new StringBuilder(source)
                .Replace(oldValue1, string.Empty)
                .Replace(oldValue2, string.Empty)
                .Replace(oldValue3, string.Empty)
                .ToString();
        }
    }
}
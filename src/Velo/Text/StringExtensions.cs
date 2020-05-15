namespace Velo.Text
{
    internal static class StringExtensions
    {
        public static string Cut(this string source, string oldValue)
        {
            return source.Replace(oldValue, string.Empty);
        }
    }
}
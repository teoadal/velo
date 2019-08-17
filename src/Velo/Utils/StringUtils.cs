using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace Velo.Utils
{
    public static class StringUtils
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string RandomString(
            this Random rnd,
            int length,
            bool includeLowercase = true,
            bool includeNumbers = true,
            bool includeUppercase = true)
        {
            var builder = new StringBuilder(length);

            // ReSharper disable StringLiteralTypo
            
            if (includeLowercase) builder.Append("abchefghjkmnpqrstuvwxyz");
            if (includeNumbers) builder.Append("0123456789");
            if (includeUppercase) builder.Append("ABCDEFGHIJKLMNOPQRSTUVWXYZ");

            // ReSharper restore StringLiteralTypo
            
            var hashLength = builder.Length;

            var result = new char[length];
            for (var i = 0; i < length; i++)
                result[i] = builder[rnd.Next(0, hashLength)];

            builder.Clear();
            
            return new string(result);
        }
    }
}
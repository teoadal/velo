using System;
using System.Collections.Generic;

namespace Velo.Utils
{
    public static class NumberUtils
    {
        public static int[] RandomSequence(this Random rnd, int from, int to, int? count = 0)
        {
            var length = to - from;
            var capacity = count ?? length;
            var result = new List<int>(capacity);
            
            for (var i = 0; i < capacity;)
            {
                var number = rnd.Next(from, to);
                if (result.Contains(number)) continue;
                
                result.Add(number);
                i++;
            }

            return result.ToArray();
        }
    }
}
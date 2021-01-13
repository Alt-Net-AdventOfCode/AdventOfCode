using System;
using System.Collections.Generic;
using System.Linq;

namespace AOCHelpers
{
    public static class DupdobArray
    {
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> array)
        {
            var rnd = new Random();
            return array.OrderBy(t => rnd.Next());
        }
    }
}
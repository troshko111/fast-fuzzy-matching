using System;
using System.Collections.Generic;
using System.Linq;

namespace FFM.SampleApp
{
    public static class IEnumerableExtensions
    {
        // http://en.wikipedia.org/wiki/Knuth_shuffle
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
        {
            var array = source.ToArray();
            var rand = new Random();

            for (var i = array.Length - 1; i > 0; i--)
            {
                var n = rand.Next(i + 1);
                var temp = array[i];
                array[i] = array[n];
                array[n] = temp;
            }

            return array;
        }
    }
}
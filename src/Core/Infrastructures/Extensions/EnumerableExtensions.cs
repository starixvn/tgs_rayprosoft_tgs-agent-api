using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TGS.Core.Infrastructures.Extensions
{
    public static class EnumerableExtensions
    {
        public static Task<T[]> WhenAll<T>(this IEnumerable<Task<T>> source) =>
            Task.WhenAll(source);

        public static IList<T[]> ToArraySegments<T>(this IEnumerable<T> source, int limit)
        {
            if (limit < 1)
            {
                throw new ArgumentException($"limit for {nameof(ToArraySegments)} cannot be less than 1");
            }

            var arrays = new List<T[]>();
            var current = new List<T>(limit);

            foreach (var e in source)
            {
                current.Add(e);
                if (current.Count != limit)
                {
                    continue;
                }

                arrays.Add(current.ToArray());
                current = new(limit);
            }

            if (current.Count > 0)
            {
                arrays.Add(current.ToArray());
            }

            return arrays;
        }
    }
}
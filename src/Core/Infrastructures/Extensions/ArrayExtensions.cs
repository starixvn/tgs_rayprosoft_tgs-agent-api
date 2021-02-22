using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TGS.Core.Infrastructures.Extensions
{
    public static class ArrayExtensions
    {
        public static async Task EachAsync<T>(this T[] source, int partitionCount, CancellationToken token,
            Func<T, CancellationToken, Task> execute)
        {
            if (partitionCount == 1)
            {
                foreach (var current in source.TakeWhile(_ => !token.IsCancellationRequested))
                {
                    await execute(current, token);
                }

                return;
            }

            await Task.WhenAll(source.GetPartitions(partitionCount).Select(partition =>
                Task.Run(async () =>
                {
                    using (partition)
                    {
                        while (partition.MoveNext())
                        {
                            if (token.IsCancellationRequested)
                            {
                                break;
                            }

                            await execute(partition.Current, token);
                        }
                    }
                }, token)));
        }

        public static async Task<TOut[]> EachAsync<TIn, TOut>(this TIn[] source, int partitionCount, 
            CancellationToken token, Func<TOut> init, Func<TIn, TOut, CancellationToken, Task> execute,
            Action<TOut> finalize = null)
        {
            if (partitionCount == 1)
            {
                var result = init();
                foreach (var current in source.TakeWhile(_ => !token.IsCancellationRequested))
                {
                    await execute(current, result, token);
                }

                finalize?.Invoke(result);
                return new[] { result };
            }

            return await Task.WhenAll(source.GetPartitions(partitionCount).Select(partition =>
                Task.Run(async () =>
                {
                    using (partition)
                    {
                        var result = init();
                        while (partition.MoveNext())
                        {
                            if (token.IsCancellationRequested)
                            {
                                break;
                            }

                            await execute(partition.Current, result, token);
                        }
                        finalize?.Invoke(result);
                        return result;
                    }
                }, token)));
        }
        
        private static IList<IEnumerator<T>> GetPartitions<T>(this T[] source, int partitionCount) => 
            Partitioner.Create(source).GetPartitions(partitionCount);
    }
}
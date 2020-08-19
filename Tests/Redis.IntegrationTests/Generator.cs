using System;

namespace TGS.Infrastructures.Redis.IntegrationTests
{
    public static class Generator
    {
        public static T[] CreateArray<T>(Func<int, T> arrayItemFactory, int count)
        {
            var result = new T[count];
            for (var i = 0; i < count; i++)
            {
                result[i] = arrayItemFactory(i);
            }

            return result;
        }
    }
}
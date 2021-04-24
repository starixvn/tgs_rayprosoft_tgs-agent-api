using System;
using Microsoft.AspNetCore.Http;

namespace TGS.Core.Infrastructures.Extensions
{
    public static class FormCollectionExtensions
    {
        public static T GetValue<T>(this IFormCollection collection, string key, T defaultValue = default)
        {
            if (!collection.ContainsKey(key))
            {
                return defaultValue;
            }

            var value = collection[key].ToString();
            return (T)Convert.ChangeType(value, typeof(T));
        }
    }
}
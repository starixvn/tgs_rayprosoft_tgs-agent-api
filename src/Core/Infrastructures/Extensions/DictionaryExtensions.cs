using System;
using System.Collections.Generic;

namespace TGS.Core.Infrastructures.Extensions
{
    public static class DictionaryExtensions
    {
        public static Guid? GetIdByKey(this Dictionary<string, string> dictionary, string key) =>
            !dictionary.TryGetValue(key, out var value) || value.IsNullOrEmpty() ||
            !Guid.TryParse(value, out var id)
                ? (Guid?) null
                : id;

        public static TEnum? GetTypeByKey<TEnum>(this Dictionary<string, string> dictionary, string key)
            where TEnum : struct =>
            !dictionary.TryGetValue(key, out var value) || value.IsNullOrEmpty() ||
            !Enum.TryParse(value, true, out TEnum resultInputType)
                ? (TEnum?) null
                : resultInputType;
    }
}
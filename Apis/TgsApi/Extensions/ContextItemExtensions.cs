using System.Collections.Generic;

namespace TGS.Apis.TgsApi.Extensions
{
    public static class ContextItemExtensions
    {
        public static T Get<T>(this IDictionary<object, object> items, object key, T defaultValue) => 
            items.TryGetValue(key, out var v) && v != null ? (T) v : defaultValue;
    }
}

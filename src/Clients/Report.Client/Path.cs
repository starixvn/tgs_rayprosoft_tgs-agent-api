using System;
using System.Linq;

namespace Report.Client
{
    public static class Path
    {
        public static string AppendWithProperties(string path, object obj) => Append(path, obj);

        public static string PathAppendWithProperties(this string path, object obj) => Append(path, obj);

        private static string Append(string path, object obj)
        {
            var last = obj.GetType().GetProperties().Last();

            return obj.GetType().GetProperties().Aggregate(path,
                (current, propertyInfo) =>
                {
                    var value = propertyInfo.GetValue(obj);

                    if (propertyInfo.PropertyType == typeof(DateTimeOffset))
                    {
                        value = ((DateTimeOffset?)value)?.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.sssZ");
                    }

                    return last.Equals(propertyInfo)
                        ? $"{current}{propertyInfo.Name}={value}"
                        : $"{current}{propertyInfo.Name}={value}&";
                });
        }
    }
}
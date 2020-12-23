using System;
using System.Reflection;
using System.Text.Json;

namespace TGS.Core.Infrastructures.Extensions
{
    public static class ObjectExtensions
    {
        public static string ToJson(this object obj, bool indented = false)
        {
            return JsonSerializer.Serialize(obj, new() { WriteIndented = indented });
        }

        public static TAttribute GetPropertyAttribute<TObject, TAttribute>(this TObject obj, string propertyName) 
            where TObject : class 
            where TAttribute : Attribute
        {
            if (propertyName.IsNullOrEmpty())
            {
                throw new ArgumentException("PropertyName can't not be null or empty");
            }

            return obj.GetType()?.GetProperty(propertyName)?.GetCustomAttribute<TAttribute>();
        }
    }
}

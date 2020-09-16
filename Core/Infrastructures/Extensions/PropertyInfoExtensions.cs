using System;
using System.Linq.Expressions;
using System.Reflection;

namespace TGS.Core.Infrastructures.Extensions
{
    public static class PropertyInfoExtensions
    {
        public static Func<object, object> CreateGetter(this PropertyInfo pi)
        {
            if (!pi.CanRead)
            {
                return null;
            }

            if (pi.DeclaringType == null)
            {
                return null;
            }

            var instance = Expression.Parameter(typeof(object), "i");
            var convertP = Expression.TypeAs(instance, pi.DeclaringType);
            var property = Expression.Property(convertP, pi);
            var convert = Expression.TypeAs(property, typeof(object));
            return (Func<object, object>)Expression.Lambda(convert, instance).Compile();
        }

        public static Action<object, object> CreateSetter(this PropertyInfo pi)
        {
            if (!pi.CanWrite)
            {
                return null;
            }

            var instance = Expression.Parameter(typeof(object), "i");
            var value = Expression.Parameter(typeof(object));
            if (pi.DeclaringType == null)
            {
                return null;
            }

            var convertedParam = Expression.Convert(instance, pi.DeclaringType);
            var propExp = Expression.Property(convertedParam, pi.Name);
            var assignExp = Expression.Assign(propExp, Expression.Convert(value, pi.PropertyType));
            return Expression.Lambda<Action<object, object>>(assignExp, instance, value).Compile();
        }
    }
}
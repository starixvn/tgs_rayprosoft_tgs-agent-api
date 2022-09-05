using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;

namespace TGS.Core.Infrastructures.Extensions;

public static class ObjectExtensions
{
    public static TAttribute GetPropertyAttribute<TObject, TAttribute>(this TObject obj, string propertyName)
        where TObject : class
        where TAttribute : Attribute
    {
        if (propertyName.IsNullOrEmpty())
        {
            throw new ArgumentException($"PropertyName({propertyName}) can't not be null or empty");
        }

        return obj.GetType()?.GetProperty(propertyName)?.GetCustomAttribute<TAttribute>();
    }

    public static string ToJson(this object obj, bool indented = false)
    {
        if (obj is null)
            return null;

        return JsonSerializer.Serialize(obj, obj.GetType(), new JsonSerializerOptions { WriteIndented = indented });
    }

    public static PropertyAccessor<T> WithPropertyExpression<T>(this T obj, Expression<Func<T, object>> expr)
    {
        if (expr is not LambdaExpression lambda)
            throw new ArgumentException("expr must be Lambda Expression.");

        if (expr.Body is MemberExpression memberExpression
            && memberExpression.Member is PropertyInfo propertyInfo)
            return new PropertyAccessor<T>(expr);

        throw new ArgumentException("Can't find property info.");
    }
}

using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

#nullable enable

namespace TGS.Core.Infrastructures.Extensions;

public class PropertyAccessor<T>
{
    private readonly Expression<Func<T, object>> _propExpr;

    internal PropertyAccessor(Expression<Func<T, object>> propExpr)
    {
        _propExpr = propExpr ?? throw new ArgumentNullException(nameof(propExpr));
    }

    public TAttribute? GetAttribute<TAttribute>()
        where TAttribute : Attribute
    {
        var memberExpression = _propExpr.Body as MemberExpression;
        var propInfo = memberExpression!.Member as PropertyInfo;

        return propInfo?.GetCustomAttribute<TAttribute>();
    }

    public string GetBindPropertyName(string defaultPropertyName)
    {
        var attribute = GetAttribute<BindPropertyAttribute>();
        return attribute?.Name ?? defaultPropertyName;
    }

    public string GetJsonPropertyName(string defaultPropertyName)
    {
        var attribute = GetAttribute<JsonPropertyNameAttribute>();
        return attribute?.Name ?? defaultPropertyName;
    }
}

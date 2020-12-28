using System.Text.Json.Serialization;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using TGS.Core.Infrastructures.Extensions;

namespace TGS.Core.Infrastructures.WebApiServices
{
    public abstract class BaseFluentValidator<T> : AbstractValidator<T> where T : class
    {
        protected string GetJsonPropertyName(T obj, string propertyName)
        {
            var attribute = obj.GetPropertyAttribute<T, JsonPropertyNameAttribute>(propertyName);
            return attribute?.Name ?? propertyName;
        }

        protected string GetBindPropertyName(T obj, string propertyName)
        {
            var attribute = obj.GetPropertyAttribute<T, BindPropertyAttribute>(propertyName);
            return attribute?.Name ?? propertyName;
        }
    }
}

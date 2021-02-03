using System;
using System.Text.Json.Serialization;

namespace TGS.Core.Models.Attributes
{
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false)]
    public sealed class InterfaceJsonConverterAttribute : JsonConverterAttribute
    {
        public InterfaceJsonConverterAttribute(Type converterType) : base(converterType)
        {
                
        }
    }
}

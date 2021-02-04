using System;
using System.Collections.Generic;
using System.Reflection;

namespace TGS.Core.Infrastructures.Extensions
{
    public class PropertyInformation
    {
        public PropertyInformation(PropertyInfo[] properties, Dictionary<string, Func<object, object>> valueGetter,
            Dictionary<string, Action<object, object>> valueSetter)
        {
            Properties = properties;
            ValueGetterByName = valueGetter;
            ValueSetterByName = valueSetter;
        }

        public PropertyInfo[] Properties { get; }
        public Dictionary<string, Func<object, object>> ValueGetterByName { get; }
        public Dictionary<string, Action<object, object>> ValueSetterByName { get; }
    }
}
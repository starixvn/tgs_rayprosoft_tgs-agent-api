using System;
using System.Collections.Generic;
using System.Reflection;

namespace TGS.Infrastructures.Extensions
{
    public class PropertyInfomation
    {
        public PropertyInfomation(PropertyInfo[] properties, Dictionary<string, Func<object, object>> valueGetter, Dictionary<string, Action<object, object>> valueSetter)
        {
            Properties = properties;
            ValueGetterByName = valueGetter;
            ValueSetterByName = valueSetter;
        }
        public PropertyInfo[] Properties { get; set; }
        public Dictionary<string, Func<object, object>> ValueGetterByName { get; set; }
        public Dictionary<string, Action<object, object>> ValueSetterByName { get; set; }
    }
}
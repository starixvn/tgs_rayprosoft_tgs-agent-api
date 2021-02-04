using System;
using System.ComponentModel;

namespace TGS.Core.Infrastructures.Extensions
{
    public static class EnumExtensions
    {
        public static string Description(this Enum value, string defaultValue = "")
        {
            var enumType = value.GetType();
            var field = enumType.GetField(value.ToString());
            var attributes = field?.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return attributes == null || attributes.Length == 0
                ? defaultValue
                : ((DescriptionAttribute) attributes[0]).Description;
        }
    }
}

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace TGS.Core.Infrastructures.Extensions
{
    public static class EnumerableExtensions
    {
        private static readonly IDictionary<Type, PropertyInfomation> EntityProperties = new ConcurrentDictionary<Type, PropertyInfomation>();

        /// <summary>
        /// Creates data table from source data.
        /// </summary>
        public static DataTable ToDataTable<T>(this IEnumerable<T> source)
        {
            var table = new DataTable();

            var properties = GetPropertyInfo(typeof(T));

            foreach (var property in properties.Properties)
            {
                table.Columns.Add(property.Name, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
            }

            object[] values = new object[properties.Properties.Length];

            foreach (var item in source)
            {
                for (int i = 0; i < properties.Properties.Length; i++)
                {
                    var cellValueGetter = properties.ValueGetterByName[properties.Properties[i].Name];
                    values[i] = cellValueGetter(item) ?? DBNull.Value;
                }

                table.Rows.Add(values);
            }

            return table;
        }

        private static PropertyInfomation GetPropertyInfo(Type type)
        {
            if (EntityProperties.TryGetValue(type, out _))
                return EntityProperties[type];

            var properties = type.GetProperties();

            var propertyGetter = properties.ToDictionary(
                property => property.Name,
                property => property.CreateGetter());

            var propertySetter = properties.ToDictionary(
                property => property.Name,
                property => property.CreateSetter());

            EntityProperties[type] = new PropertyInfomation(properties, propertyGetter, propertySetter);

            return EntityProperties[type];
        }
    }
}
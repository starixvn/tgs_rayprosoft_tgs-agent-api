using System;
using System.Collections.Generic;
using System.Data;
using Dapper;
using Npgsql;
using NpgsqlTypes;

namespace TGS.Core.DataServices.Base.CustomQueryParameters
{
    public sealed class ArrayParameter<T> : SqlMapper.ICustomQueryParameter
    {
        private readonly IEnumerable<T> _value;

        public ArrayParameter(IEnumerable<T> value)
        {
            _value = value;
        }

        void SqlMapper.ICustomQueryParameter.AddParameter(IDbCommand command, string name)
        {
            var type = GetType<T>();
            command.Parameters.Add(new NpgsqlParameter(name, type)
            {
                Value = _value
            });
        }

        private static NpgsqlDbType GetType<T1>()
        {
            var type = typeof(T1);
            if (typeof(T1) == typeof(Guid))
            {
                return NpgsqlDbType.Array | NpgsqlDbType.Uuid;
            }

            return Type.GetTypeCode(type) switch
            {
                TypeCode.Int16 or TypeCode.Int32 => NpgsqlDbType.Array | NpgsqlDbType.Integer,
                TypeCode.Boolean => NpgsqlDbType.Array | NpgsqlDbType.Boolean,
                TypeCode.String => NpgsqlDbType.Array | NpgsqlDbType.Text,
                TypeCode.Decimal => NpgsqlDbType.Array | NpgsqlDbType.Numeric,
                _ => throw new InvalidOperationException($"unsupported type '{type.Name}' for npgsql array")
            };
        }
    }
}

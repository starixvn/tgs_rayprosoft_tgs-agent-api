using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TGS.Core.Infrastructures.Json.JsonConverters
{
    public sealed class NumberToStringConverter : JsonConverter<string>
    {
        public override string Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.Number && type == typeof(string))
            {
                return reader.GetString();
            }

            var v = reader.GetDecimal();
            return Convert.ToString(v, CultureInfo.InvariantCulture);
        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options) =>
            writer.WriteStringValue(value);
    }
}
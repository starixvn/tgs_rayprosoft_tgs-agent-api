using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.IMESports
{
    public class StringToNumberConverter : JsonConverter<object>
    {
        public override bool CanConvert(Type typeToConvert)
        {
            // see https://stackoverflow.com/questions/1749966/c-sharp-how-to-determine-whether-a-type-is-a-number
            switch (Type.GetTypeCode(typeToConvert))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                    return true;
                default:
                    return false;
            }
        }

        public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                var s = reader.GetString();
                return int.TryParse(s, out var i)
                    ? i
                    : (decimal.TryParse(s, out var d) ? d : throw new Exception($"unable to parse {s} to number")
                    );
            }

            if (reader.TokenType == JsonTokenType.Number)
            {
                return reader.TryGetInt64(out long l) ? l : reader.GetDecimal();
            }

            using (JsonDocument document = JsonDocument.ParseValue(ref reader))
            {
                throw new Exception($"unable to parse {document.RootElement.ToString()} to number");
            }
        }


        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            var str = value.ToString();
            if (int.TryParse(str, out var i))
            {
                writer.WriteNumberValue(i);
            }
            else if (decimal.TryParse(str, out var d))
            {
                writer.WriteNumberValue(d);
            }
            else
            {
                throw new Exception($"unable to parse {str} to number");
            }
        }
    }
}
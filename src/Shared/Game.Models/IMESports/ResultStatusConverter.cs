using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using TGS.Shared.Game.Models.IMESports.Enums;

namespace TGS.Shared.Game.Models.IMESports
{
    public class ResultStatusConverter : JsonConverter<ResultStatus>
    {
        public override ResultStatus
            Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();

            if (string.IsNullOrEmpty(value))
            {
                return ResultStatus.None;
            }

            return Enum.TryParse<ResultStatus>(value, out var result) ? result : ResultStatus.Unknown;
        }

        public override void Write(Utf8JsonWriter writer, ResultStatus value, JsonSerializerOptions options) =>
            writer.WriteStringValue(value.ToString());
    }
}
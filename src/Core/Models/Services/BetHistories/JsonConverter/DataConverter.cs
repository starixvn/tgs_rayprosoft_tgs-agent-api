using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TGS.Core.Models.Services.BetHistories.JsonConverter
{
    public class DataConverter : JsonConverter<IData>
    {
        public override IData Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return (IData) JsonSerializer.Deserialize(ref reader, typeToConvert, options);
        }

        public override void Write(Utf8JsonWriter writer, IData value, JsonSerializerOptions options)
        {
            switch (value)
            {
                case null:
                    JsonSerializer.Serialize(writer, (IData) null, options);
                    break;

                default:
                {
                    var type = value.GetType();
                    JsonSerializer.Serialize(writer, value, type, options);
                    break;
                }
            }
        }
    }
}

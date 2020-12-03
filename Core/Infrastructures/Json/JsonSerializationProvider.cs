using System;
using System.Text.Json;

namespace TGS.Core.Infrastructures.Json
{
    public interface IJsonSerializationProvider
    {
        T DeserializeFromString<T>(string value, JsonSerializerOptions options = null);

        string SerializeToString<T>(T obj, JsonSerializerOptions options = null);

        string SerializeToString(object obj, Type inputType, JsonSerializerOptions options = null);
    }

    public sealed class JsonSerializationProvider : IJsonSerializationProvider
    {
        T IJsonSerializationProvider.DeserializeFromString<T>(string value, JsonSerializerOptions options) =>
            JsonSerializer.Deserialize<T>(value, options);

        string IJsonSerializationProvider.SerializeToString<T>(T obj, JsonSerializerOptions options) =>
            JsonSerializer.Serialize(obj, options);

        string IJsonSerializationProvider.SerializeToString(object obj, Type inputType, JsonSerializerOptions options) =>
            JsonSerializer.Serialize(obj, inputType, options);
    }
}
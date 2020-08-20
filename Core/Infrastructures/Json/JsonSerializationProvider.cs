using System.Text.Json;

namespace TGS.Infrastructures.Json
{
    public interface IJsonSerializationProvider
    {
        T DeserializeFromString<T>(string value, JsonSerializerOptions options = null);

        string SerializeToString<T>(T obj, JsonSerializerOptions options = null);
    }

    public sealed class JsonSerializationProvider : IJsonSerializationProvider
    {
        T IJsonSerializationProvider.DeserializeFromString<T>(string value, JsonSerializerOptions options)
        {
            return JsonSerializer.Deserialize<T>(value, options);
        }

        string IJsonSerializationProvider.SerializeToString<T>(T obj, JsonSerializerOptions options)
        {
            return JsonSerializer.Serialize<T>(obj, options);
        }
    }
}

using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TGS.Infrastructures.Json
{
    public interface IJsonSerializationProvider
    {
        T DeserializeFromString<T>(string value, JsonSerializerSettings settings = null);

        dynamic DeserializeFromString(string value, Type objectType, JsonSerializerSettings settings = null);

        dynamic DeserializeAsDynamic(string value, JsonLoadSettings settings = null);

        string SerializeToString<T>(T obj, JsonSerializerSettings settings = null);
    }

    public sealed class JsonSerializationProvider : IJsonSerializationProvider
    {
        T IJsonSerializationProvider.DeserializeFromString<T>(string value, JsonSerializerSettings settings)
        {
            return JsonConvert.DeserializeObject<T>(value, settings);
        }

        dynamic IJsonSerializationProvider.DeserializeFromString(string value, Type objectType, JsonSerializerSettings settings)
        {
            return JsonConvert.DeserializeObject(value, objectType, settings);
        }

        dynamic IJsonSerializationProvider.DeserializeAsDynamic(string value, JsonLoadSettings settings)
        {
            return JObject.Parse(value, settings);
        }

        string IJsonSerializationProvider.SerializeToString<T>(T obj, JsonSerializerSettings settings)
        {
            return JsonConvert.SerializeObject(obj, settings);
        }
    }
}

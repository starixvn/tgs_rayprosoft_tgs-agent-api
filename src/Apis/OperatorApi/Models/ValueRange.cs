using System;
using System.Text.Json.Serialization;

namespace TGS.Apis.OperatorApi.Models
{
    public struct ValueRange<T> where T : IComparable<T>
    {
        [JsonPropertyName("from")]
        public T From { get; init; }

        [JsonPropertyName("to")]
        public T To { get; init; }

        public void Deconstruct(out T from, out T to)
        {
            from = From;
            to = To;
        }
    }
}

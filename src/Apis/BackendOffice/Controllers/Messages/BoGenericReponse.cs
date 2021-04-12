using System.Text.Json.Serialization;
using TGS.Core.Models.Messages;

namespace TGS.Apis.BackendOffice.Controllers.Messages
{
    public record BoGenericReponse<TData> : TgsResponse
    {
        [JsonPropertyName("data")]
        public TData Data { get; set; }
    }
}

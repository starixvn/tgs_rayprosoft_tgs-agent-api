// ReSharper disable IdentifierTypo

using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.IMESports.Outgoing
{
    public record NewLaunchGameResponse : BaseResponse
    {
        [JsonPropertyName("GameUrl")]
        public string GameUrl { get; init; }
    }
}

// ReSharper disable IdentifierTypo

using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.IMESports.Outgoing
{
    public record NewLaunchGameRequest : BasePlayerRequest
    {
        [JsonPropertyName("GameCode")]
        public string GameId { get; init; }
        
        [JsonPropertyName("Language")]
        public string Language { get; init; }

        [JsonPropertyName("IpAddress")] public string IpAddress = "127.0.0.1";
        
        [JsonPropertyName("ProductWallet")]
        public int ProductWallet { get; init; }
        
        [JsonPropertyName("Http")]
        public int Http { get; init; }
    }
}

using System.Text.Json.Serialization;
using MediatR;

namespace TGS.Apis.GlobalApi.Models.Messages
{
    public record CreatePlayerAndLoginRequest : IRequest<CreatePlayerAndLoginResponse>
    {
        [JsonPropertyName("brandcode")]
        public string BrandCode { get; init; }

        [JsonPropertyName("username")]
        public string Username { get; init; }

        [JsonPropertyName("currencycode")]
        public string CurrencyCode { get; init; }

        [JsonPropertyName("initwalletamount")]
        public decimal InitialWalletAmount { get; init; }
    }
}
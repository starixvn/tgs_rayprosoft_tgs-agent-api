// ReSharper disable IdentifierTypo

using System.Text.Json.Serialization;

namespace TGS.Apis.TgsApi.GameModels.SBOBET.Outgoing
{
    public record RegisterPlayerRequest : BasePlayerRequest
    {
        private readonly string _brandCode;
        private readonly string _currencyCode;

        public RegisterPlayerRequest(string brandCode, string currencyCode)
        {
            _brandCode = brandCode;
            _currencyCode = currencyCode;
        }

        /// <summary>
        /// As per SBO, a agent can only have 1 currency
        /// </summary>
        [JsonPropertyName("Agent")]
        public string Agent => $"{_brandCode}{_currencyCode}";
    }
}

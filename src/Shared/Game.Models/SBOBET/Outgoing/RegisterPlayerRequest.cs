// ReSharper disable IdentifierTypo

using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.SBOBET.Outgoing
{
    public record RegisterPlayerRequest : BasePlayerRequest
    {
        private readonly string _agentPrefix;
        private readonly string _currencyCode;

        public RegisterPlayerRequest(string agentPrefix, string currencyCode)
        {
            _agentPrefix = agentPrefix;
            _currencyCode = currencyCode;
        }

        /// <summary>
        ///     As per SBO, a agent can only have 1 currency
        /// </summary>
        [JsonPropertyName("Agent")]
        public string Agent => $"{_agentPrefix}{_currencyCode.Replace("_", string.Empty)}";
    }
}
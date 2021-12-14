using System;
using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.BetConstruct.Incoming
{
    public record GetClientDetailsResponse : ErrorResponse
    {
        [JsonPropertyName("Login")]
        public string Login { get; set; }

        [JsonPropertyName("CurrencyId")]
        public string CurrencyId { get; set; }

        [JsonPropertyName("LanguageId")]
        public string LanguageId { get; set; }

        [JsonPropertyName("Email")]
        public string Email { get; set; }

        [JsonPropertyName("BirthDate")]
        public DateTimeOffset? BirthDate { get; set; }

        [JsonPropertyName("Gender")]
        public int Gender { get; set; }

        [JsonPropertyName("ExternalId")]
        public string ExternalId { get; set; }

        [JsonPropertyName("CurrentIp")]
        public string CurrentIp { get; set; }

        [JsonPropertyName("Phone")]
        public string Phone { get; set; }

        [JsonPropertyName("Address")]
        public string Address { get; set; }

        [JsonPropertyName("ExternalBonusId")]
        public int? ExternalBonusId { get; set; }

        [JsonPropertyName("IsTest")]
        public bool IsTest { get; set; }

        [JsonPropertyName("CountryId")]
        public string CountryId { get; set; }

        [JsonPropertyName("PartnerFlag")]
        public string PartnerFlag { get; set; }
    }
}

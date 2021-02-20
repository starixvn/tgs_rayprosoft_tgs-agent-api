using System;
using System.Text.Json.Serialization;
using MediatR;

namespace TGS.Services.Report.Application.Models.Round
{
    public class GetRoundDetailRequest : IRequest<RoundDetailModel>
    {
        [JsonPropertyName("gpcode")] public string GameProviderCode { get; init; }

        [JsonPropertyName("playercode")] public string PlayerCode { get; init; }

        [JsonPropertyName("roundid")] public Guid RoundId { get; init; }

        /// <summary>
        ///     player's culture code
        /// </summary>
        [JsonPropertyName("language")]
        public string Language { get; init; }
    }
}
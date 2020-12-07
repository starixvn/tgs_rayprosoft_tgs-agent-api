using System;

namespace TGS.Core.Models.Messages.Pluggables
{
    public record GetRoundHistoryRequest
    {
        public string GameProviderCode { get; init; }
        public string PlayerCode { get; init; }
        public Guid RoundId { get; init; }
        public string Language { get; init; }
    }
}
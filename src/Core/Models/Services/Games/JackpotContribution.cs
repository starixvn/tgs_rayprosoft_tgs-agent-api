using System.Text.Json.Serialization;

namespace TGS.Core.Models.Services.Games
{
    public record JackpotContribution
    {
        public string JackpotId { get; init; }
        public decimal TotalAmount { get; init; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Pool[] Pools { get; init; }

        public record Pool(string Id, decimal Amount);
    }
}
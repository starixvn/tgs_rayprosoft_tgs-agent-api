using System.Text.Json.Serialization;
using TGS.Shared.Game.Models.TCG.Common;

namespace TGS.Shared.Game.Models.TCG.Incoming
{
    public record TCGCheckTransactionStatusResponse : TCGSingleWalletResponse
    {
        [JsonPropertyName("query_result")]
        public QueryResult[] QueryResult { get; init; }
    }
}

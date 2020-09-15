using TGS.Core.Models.Services.Caches;
using TGS.Core.Models.Services.Games;

namespace TGS.Services.Operations.Models.Data
{
    internal record GameActionResult
    {
        public TransactionResult TransactionResult { get; init; }
        public GameActionData GameAction { get; internal set; }
        public BetActionData ReferencedGameAction { get; internal set; }
        public CachedGame Game { get; internal set; }
    }
}

// unset

namespace TGS.Shared.Game.Models.JDB.Incoming
{
    public record CancelTransactionRequest : BaseActionRequest
    {
        public long TransferId { get; set; }
    }
}
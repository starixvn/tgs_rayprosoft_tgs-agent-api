// unset

namespace TGS.Shared.Game.Models.JDB.Incoming
{
    public record BetHistoryResponse : BaseResponse
    {
        public BetHistoryPageUrl[] Data { get; init; }
    }

    public record BetHistoryPageUrl
    {
        public string Path { get; init; }
    }
}
// unset

namespace TGS.Shared.Game.Models.JDB.Incoming
{
    public class BetHistoryResponse : BaseResponse
    {
        public BetHistoryPageUrl[] Data { get; set; }
    }
    
    public class BetHistoryPageUrl 
    {
        public string Path { get; set; }
    }
}
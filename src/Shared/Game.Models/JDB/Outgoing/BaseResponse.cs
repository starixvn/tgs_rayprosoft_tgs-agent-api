// unset

namespace TGS.Shared.Game.Models.JDB.Outgoing
{
    public class BaseResponse
    {
        public string Status { get; set; }
        public decimal Balance { get; set; }
        public string ErrorMessage { get; set; }
    }
}
namespace TGS.Shared.Game.Models.PragmaticPlay.Incoming
{
    public record GetBalanceRequest : BaseRequest
    {
        public string UserId { get; init; }
    }
}
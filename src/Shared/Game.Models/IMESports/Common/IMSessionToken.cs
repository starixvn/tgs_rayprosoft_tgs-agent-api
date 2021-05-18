namespace TGS.Shared.Game.Models.IMESports.Common
{
    public record IMSessionToken
    {
        public string SessionToken { get; init; }
        public int TimeToLeave { get; init; }
    }
}
namespace TGS.Shared.Game.Models.PragmaticPlay.Incoming
{
    public record BaseRequest : HashRequest
    {
        public string Token { get; set; }

        public string ProviderId { get; set; }

        public bool BypassHashVerify { get; set; }
    }
}
namespace TGS.Core.Models.GameProviders.Settings.DreamGame
{
    public record DreamGameBetLimit()
    {
        public string Currency { get; init; }

        public string TableLimit { get; init; }
    }
}

namespace TGS.Core.Models.Services.Games
{
    public record GameActionData
    {
        public GameActionRecord[] GameActions { get; init; }

        public RoundRecord Round { get; init; }
    }
}
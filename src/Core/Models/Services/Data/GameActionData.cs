namespace TGS.Core.Models.Services.Data
{
    public record GameActionData
    {
        public GameActionRecord[] GameActions { get; init; }

        public RoundRecord Round { get; init; }
    }
}
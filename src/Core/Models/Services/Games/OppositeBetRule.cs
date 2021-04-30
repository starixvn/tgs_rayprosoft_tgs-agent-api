namespace TGS.Core.Models.Services.Games
{
    public record OppositeBetRule
    {
        public string[] ExternalGameIds { get; init; }

        public Pattern[] Patterns { get; init; }

        public record Pattern(string BetType, int GroupId);
    }
}
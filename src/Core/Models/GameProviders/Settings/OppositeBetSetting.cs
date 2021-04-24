namespace TGS.Core.Models.GameProviders.Settings
{
    public record OppositeBetSetting
    {
        // game external ids
        public string[] Games { get; init; }

        public Pattern[] Patterns { get; init; }

        public record Pattern(string BetType, int GroupId);
    }
}
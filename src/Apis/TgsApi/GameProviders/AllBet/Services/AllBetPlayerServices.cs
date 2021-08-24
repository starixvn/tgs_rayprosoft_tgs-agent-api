namespace TGS.Apis.TgsApi.GameProviders.AllBet.Services
{
    public interface IAllBetPlayerServices
    {
        string GetPlayerCode(string allBetPlayerId, string suffix);

        string ToAllBetPlayerId(string playerCode, string suffix);
    }

    public class AllBetPlayerServices : IAllBetPlayerServices
    {
        public string GetPlayerCode(string allBetPlayerId, string suffix)
            => allBetPlayerId.Replace($"_{suffix}", string.Empty);

        public string ToAllBetPlayerId(string playerCode, string suffix)
            => $"{playerCode}_{suffix}";
    }
}
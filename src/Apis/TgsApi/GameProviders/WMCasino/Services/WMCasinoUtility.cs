namespace TGS.Apis.TgsApi.GameProviders.WMCasino.Services
{
    public interface IWMCasinoUtility
    {
        public string GetExternalRoundIdByGameNumber(string gameNumber, string externalGameId);

        public string GetExternalRoundIdByBetResult(string roundId, string subRoundId);
    }

    public class WMCasinoUtility : IWMCasinoUtility
    {
        public string GetExternalRoundIdByGameNumber(string gameNumber, string externalGameId)
            => gameNumber.Replace($"{externalGameId}_", string.Empty);

        public string GetExternalRoundIdByBetResult(string roundId, string subRoundId)
            => $"{roundId}_{subRoundId}";
    }
}
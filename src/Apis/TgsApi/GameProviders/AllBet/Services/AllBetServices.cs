namespace TGS.Apis.TgsApi.GameProviders.AllBet.Services
{
    public interface IAllBetServices
    {
        string GetPlayerCode(string allBetPlayerId, string suffix);
    }

    public class AllBetServices : IAllBetServices
    {
        string IAllBetServices.GetPlayerCode(string allBetPlayerId, string suffix)
            => allBetPlayerId.Replace(suffix, string.Empty);
    }
}
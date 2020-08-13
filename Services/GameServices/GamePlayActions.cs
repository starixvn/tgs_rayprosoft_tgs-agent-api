using System.Threading.Tasks;

namespace TGS.Services.GameServices
{
    public interface IGamePlayActions
    {
        Task ValidatePlayerAsync();

        Task GetBalanceAsync(string gameProviderCode, string playerCode);

        // Task PlaceBetAsync(BetData data);

    }

    public class GamePlayActions
    {
    }
}

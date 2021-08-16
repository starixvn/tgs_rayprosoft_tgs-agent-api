using System.Threading.Tasks;
using TGS.Core.Models;
using TGS.Core.Models.Services.Games;
using TGS.Services.GameOperations;

namespace TGS.Apis.TgsApi.GameProviders.WMCasino.Services
{
    public interface IWMCasinoServices
    {
        ValueTask<GetBalanceResult> GetBalanceAsync(string playerCode);
    }

    public class WMCasinoServices : IWMCasinoServices
    {
        private static readonly string _GameProviderCode = Consts.GameProvider.WMCasino;
        private readonly IGamePlayActions _actions;

        public WMCasinoServices(IGamePlayActions actions)
        {
            _actions = actions;
        }

        public ValueTask<GetBalanceResult> GetBalanceAsync(string playerCode)
            => _actions.GetBalanceAsync(_GameProviderCode, playerCode);
    }
}
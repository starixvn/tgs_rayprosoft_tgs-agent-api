using System.Threading.Tasks;
using TGS.Shared.Game.Models.SBOBET.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.SBOBET.Factories
{
    public interface IBetService
    {
        ValueTask<GetBalanceResponse> GetBalanceAsync(GetBalanceRequest request);

        ValueTask<PlaceBetResponse> PlaceBetAsync(PlaceBetRequest request);

        ValueTask<SettleBetResponse> SettleBetAsync(SettleBetRequest request);

        ValueTask<CancelBetResponse> CancelBetAsync(CancelBetRequest request);

        ValueTask<RollbackResponse> RollbackAsync(RollbackRequest request);

        ValueTask<GetBetStatusResponse> GetBetStatusAsync(GetBetStatusRequest request);
    }
}
using System;
using System.Threading.Tasks;
using TGS.Shared.Game.Models.SBOBET.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.SBOBET.Factories
{
    internal class OthersBetService : IBetService
    {
        ValueTask<CancelBetResponse> IBetService.CancelBetAsync(CancelBetRequest request) =>
            throw new NotImplementedException();

        ValueTask<GetBalanceResponse> IBetService.GetBalanceAsync(GetBalanceRequest request) =>
            throw new NotImplementedException();

        ValueTask<GetBetStatusResponse> IBetService.GetBetStatusAsync(GetBetStatusRequest request) =>
            throw new NotImplementedException();

        ValueTask<PlaceBetResponse> IBetService.PlaceBetAsync(PlaceBetRequest request) =>
            throw new NotImplementedException();

        ValueTask<RollbackResponse> IBetService.RollbackAsync(RollbackRequest request) =>
            throw new NotImplementedException();

        ValueTask<SettleBetResponse> IBetService.SettleBetAsync(SettleBetRequest request) =>
            throw new NotImplementedException();
    }
}
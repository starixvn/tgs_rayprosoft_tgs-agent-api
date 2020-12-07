// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming

using System.Threading.Tasks;
using TGS.Apis.TgsApi.Models.Interfaces;
using TGS.Core.Models.Messages.Pluggables;

namespace TGS.Apis.TgsApi.GameProviders.SBOBET.Services
{

    public sealed class SBOBETGameHistory : IGameHistory
    {
        Task<GetRoundHistoryResponse> IGameHistory.GetRoundHistoryAsync(GetRoundHistoryRequest request)
        {
            return Task.FromResult(new GetRoundHistoryResponse());
        }
    }
}

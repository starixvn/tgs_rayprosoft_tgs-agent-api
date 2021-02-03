using System.Threading.Tasks;
using TGS.Services.PluggableServices.Models;

namespace TGS.Apis.TgsApi.Models.Interfaces
{
    public interface IGameHistory
    {
        Task<GetRoundHistoryResponse> GetRoundHistoryAsync(GetRoundHistoryRequest request);
    }
}

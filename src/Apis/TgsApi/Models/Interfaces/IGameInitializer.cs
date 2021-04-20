using System.Threading.Tasks;
using TGS.Services.PluggableServices.Models;

namespace TGS.Apis.TgsApi.Models.Interfaces
{
    public interface IGameInitializer
    {
        ValueTask<GetGameUrlResponse> GetGameUrlAsync(GetGameUrlRequest request);
    }
}
using System.Threading.Tasks;
using TGS.Core.Models.Messages.Pluggables;

namespace TGS.Apis.TgsApi.Models.Interfaces
{
    public interface IGameInitializer
    {
        Task<GetGameUrlResponse> GetGameUrlAsync(GetGameUrlRequest request);
    }
}

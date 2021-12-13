using System.Threading.Tasks;
using TGS.Core.Models.GameProviders;

namespace TGS.Apis.TgsApi.GameProviders.Jili.Services
{
    public interface IJiliService
    {
        ValueTask<JiliConfiguration> GetConfigurationAsync();
    }
}
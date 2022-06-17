using System.Threading.Tasks;

namespace TGS.Apis.TgsApi.GameProviders.TCG.Services;

public interface ITCGCacheService
{
    public ValueTask AddAsync(string userName, string refNo);

    public Task<bool> ExistAsync(string userName, string refNo);

    public ValueTask RemoveAsync(string userName, string refNo);
}

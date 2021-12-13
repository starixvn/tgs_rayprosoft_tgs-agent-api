using System;
using System.Threading.Tasks;

namespace TGS.Services.Agent
{
    public interface IBrandRepository
    {
        ValueTask<BrandInfo?> GetBrandInfoAsync(Guid brandId);
    }
}

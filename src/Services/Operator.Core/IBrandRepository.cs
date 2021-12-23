using System;
using System.Threading.Tasks;

namespace TGS.Services.Operator
{
    public interface IBrandRepository
    {
        ValueTask<BrandInfo?> GetBrandInfoAsync(Guid brandId);
    }
}

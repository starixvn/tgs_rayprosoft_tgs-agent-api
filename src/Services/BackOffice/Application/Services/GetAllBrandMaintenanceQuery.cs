using TGS.Services.BackOffice.Domain;

namespace TGS.Services.BackOffice.Application.Services
{
    public record GetAllBrandMaintenanceQuery(
        string BrandCode,
        Paging.Paging<BrandMaintenanceInfo> Paging);
}

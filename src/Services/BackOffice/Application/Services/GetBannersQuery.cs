using System;
using TGS.Services.BackOffice.Domain;

namespace TGS.Services.BackOffice.Application.Services
{
    public record GetBannersQuery(
        Guid BrandId,
        BannerStatus BannerStatus);
}

using System;

namespace TGS.Services.BackOffice.Application.Services
{
    public record CheckBrandEmailQuery(Guid BrandId, string Email);
}

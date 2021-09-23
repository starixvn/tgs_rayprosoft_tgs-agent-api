using System;

namespace TGS.Services.Operator.Services.Player
{
    public record CheckBrandEmailQuery(Guid BrandId, string Email, string PlayerCode);
}

using System;

namespace TGS.Services.Operator.Services.Player
{
    public record CheckBrandEmailCommand(Guid BrandId, string Email);
}

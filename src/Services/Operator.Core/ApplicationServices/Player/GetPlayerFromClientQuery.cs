using System;

namespace TGS.Services.Operator.ApplicationServices.Player
{
    public record GetPlayerFromClientQuery(Guid BrandId, string PlayerCode);
}

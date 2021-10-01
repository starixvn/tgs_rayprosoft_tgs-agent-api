using System;

namespace TGS.Services.Operator.Services.Player
{
    public record CheckPlayerBannedQuery(Guid BrandId, string UserName);
}
using System;

namespace TGS.Services.Operator.Services.Player
{
    public record CheckPlayerBannedCommand(Guid BrandId, string UserName);
}
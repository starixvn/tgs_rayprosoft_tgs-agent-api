using System;

namespace TGS.Services.Operator.Services.Player
{
    public record GetPlayerInfoByUserNameQuery(Guid BrandId, string UserName);
}

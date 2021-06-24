using System;

namespace TGS.Services.Operator.Services.Player
{
    public record GetPlayerQuery(Guid BrandId, string PlayerCode);
}
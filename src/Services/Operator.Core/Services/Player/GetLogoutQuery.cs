using System;
using TGS.Core.Models.Tokens;

namespace TGS.Services.Operator.Services.Player
{
    public record GetLogoutQuery(Guid BrandId, PlayerTokenData PlayerTokenData);
}
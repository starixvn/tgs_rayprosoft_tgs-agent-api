using System;
using TGS.Core.Models.Tokens;

namespace TGS.Services.Operator.ApplicationServices.Player
{
    public record GetLogoutQuery(Guid BrandId, PlayerTokenData PlayerTokenData);
}

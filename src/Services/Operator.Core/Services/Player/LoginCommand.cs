using System;
using TGS.Core.Models.Enums;

namespace TGS.Services.Operator.Services.Player
{
    public record LoginCommand(
        Guid BrandId,
        string UserName,
        string Password,
        string IpAddress,
        PlatformType PlatformType,
        string Site);
}
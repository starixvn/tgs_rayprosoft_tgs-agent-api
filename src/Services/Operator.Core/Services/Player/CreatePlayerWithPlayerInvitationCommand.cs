using System;

#nullable enable

namespace TGS.Services.Operator.Services.Player
{
    public record CreatePlayerWithPlayerInvitationCommand(
        Guid BrandId,
        string UserName,
        string Password,
        string CurrencyCode,
        string UpperPlayerCode,
        bool IsTestPlayer,
        string IpAddress,
        string? HostName);
}

using System;

namespace TGS.Services.Operator.Services.Player
{
    public record CreatePlayerCommand(
        Guid BrandId,
        string UserName,
        string Password,
        string CurrencyCode,
        string AgentInvitationCode,
        bool IsTestPlayer,
        string IpAddress);
}
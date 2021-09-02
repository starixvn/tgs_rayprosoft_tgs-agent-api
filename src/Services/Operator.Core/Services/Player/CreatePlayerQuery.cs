using System;

namespace TGS.Services.Operator.Services.Player
{
    public record CreatePlayerQuery(
        Guid BrandId,
        string UserName,
        string Password,
        string CurrencyCode,
        string AgentInvitationCode,
        bool IsTestPlayer);
}
using System;

namespace TGS.Services.Operator.Services.Player
{
    public record GetCreatePlayerQuery(
        Guid BrandId,
        string UserName,
        string Password,
        string CurrencyCode,
        string AgentInvitationCode,
        bool IsTestPlayer);
}
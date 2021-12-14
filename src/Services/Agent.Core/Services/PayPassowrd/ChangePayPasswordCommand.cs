using System;

namespace TGS.Services.Agent.Services.PayPassowrd
{
    public record ChangePayPasswordCommand(
        Guid AgentId,
        string OldPassword,
        string NewPassword);
}

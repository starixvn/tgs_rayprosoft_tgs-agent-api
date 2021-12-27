using System;

namespace TGS.Services.Agent.ApplicationServices.PayPassowrd
{
    public record ChangePayPasswordCommand(
        Guid AgentId,
        string OldPassword,
        string NewPassword);
}

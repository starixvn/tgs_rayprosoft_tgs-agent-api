using System;

namespace TGS.Services.Agent.Services.PayPassowrd
{
    public record SetFirstPayPasswordCommand(Guid AgentId, string Password);
}

using System;
using System.Globalization;

namespace TGS.Services.Agent.Services.Agent
{
    public record StartSetAgentEmailCommand(Guid AgentId, string Email, CultureInfo CultureInfo);
}

using System;

namespace TGS.Services.Agent.ApplicationServices.Agent;

public record SetAgentPasswordCommand(Guid AgentId, string NewPassword);
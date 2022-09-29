using System;

namespace TGS.Services.Agent.ApplicationServices.PayPassowrd;

public record SetFirstPayPasswordCommand(Guid AgentId, string Password);

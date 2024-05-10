using System;

namespace TGS.Services.Agent.ApplicationServices.Player;

public record SetPlayerEmailCommand(Guid AgentId, string PlayerCode, string Email);
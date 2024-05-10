using System;

namespace TGS.Services.Agent.ApplicationServices.Player;

public record SetPlayerPhoneCommand(Guid AgentId, string PlayerCode, string AreaCode, string Phone);
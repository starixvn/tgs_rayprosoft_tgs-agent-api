using System;

namespace TGS.Apis.AgentApi.Models;

public record ResetSubAgentPasswordRequest(Guid SubAgentId, string Password);
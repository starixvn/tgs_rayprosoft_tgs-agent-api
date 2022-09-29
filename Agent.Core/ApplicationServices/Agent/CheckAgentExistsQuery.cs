using System;

namespace TGS.Services.Agent.ApplicationServices.Agent;

public record CheckAgentExistsQuery(Guid BrandId, string Username);

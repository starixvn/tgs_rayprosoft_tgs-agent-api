using System;

namespace TGS.Services.Agent.Services.Agent
{
    public record CheckAgentExistsQuery(Guid BrandId, string Username);
}
namespace TGS.Services.Agent
{
    public class CreateAgentInfo
    {
        public string AgentId { get; init; } = default!;

        public bool IsNew { get; init; }

        public string Message { get; init; } = default!;
    }
}

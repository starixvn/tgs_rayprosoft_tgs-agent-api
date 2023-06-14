namespace TGS.Services.Agent;

public record AgentWinLossBreadcrumb
{
    public string Username { get; set; } = default!;
    public string Id { get; set; } = default!;
    public string MemberType { get; set; } = default!;
}
namespace TGS.Core.Models.GameProviders.Settings.LY;

/// <summary>
/// LY Agent Mapper
/// </summary>
/// <seealso cref="System.IEquatable&lt;TGS.Core.Models.GameProviders.Settings.LY.LYAgentMapper&gt;" />
public record LYAgentMapper
{
    public string CurrencyCode { get; set; }

    public string AgentId { get; set; }

    public bool Status { get; set; }
}

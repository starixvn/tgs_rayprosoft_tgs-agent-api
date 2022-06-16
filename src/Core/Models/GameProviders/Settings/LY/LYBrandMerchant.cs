using System;

namespace TGS.Core.Models.GameProviders.Settings.LY;

/// <summary>
/// LY Brand 商戶
/// </summary>
public record LYBrandMerchant
{
    /// <summary>
    /// The brand code
    /// </summary>
    public string BrandCode { get; set; }

    /// <summary>
    /// The API URL
    /// </summary>
    public Uri ApiUrl { get; set; }

    /// <summary>
    /// The API record URL
    /// </summary>
    public Uri ApiRecordUrl { get; set; }

    /// <summary>
    /// The deskey
    /// </summary>
    public string Deskey { get; set; }

    /// <summary>
    /// The md5key
    /// </summary>
    public string Md5key { get; set; }

    /// <summary>
    /// The agent mapper
    /// </summary>
    public LYAgentMapper[] AgentMappers { get; set; }

    /// <summary>
    /// The status
    /// </summary>
    public bool Status { get; set; }
}

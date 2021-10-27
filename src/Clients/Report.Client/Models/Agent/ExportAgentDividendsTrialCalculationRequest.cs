using System;

namespace TGS.Clients.Report.Client.Models.Agent
{
    public record ExportAgentDividendsTrialCalculationRequest
        (string BrandCode, DateTimeOffset? From, DateTimeOffset? To);
}
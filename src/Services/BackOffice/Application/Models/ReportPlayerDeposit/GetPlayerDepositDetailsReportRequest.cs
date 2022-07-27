using System;
using Tgs.Report.Client.Models;

namespace TGS.Services.BackOffice.Application.Models.ReportPlayerDeposit;

public record GetPlayerDepositDetailsReportRequest : PageableRequest
{
    public Guid BrandId { get; init; }
    public string AgentUsername { get; init; } = default!;
    public DateOnly Date { get; init; }
}

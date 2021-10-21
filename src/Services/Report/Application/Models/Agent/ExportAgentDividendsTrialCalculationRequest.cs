using System;
using System.IO;
using MediatR;

namespace TGS.Services.Report.Application.Models.Agent
{
    public record ExportAgentDividendsTrialCalculationRequest
        (string BrandCode, DateTimeOffset? From, DateTimeOffset? To) : IRequest<MemoryStream>;
}
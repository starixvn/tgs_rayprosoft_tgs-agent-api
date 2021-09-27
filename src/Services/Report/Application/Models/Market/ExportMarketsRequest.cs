using System;
using System.IO;
using MediatR;

namespace TGS.Services.Report.Application.Models.Market
{
    public record ExportMarketsRequest(Guid BrandId, DateTimeOffset From, DateTimeOffset To) : IRequest<MemoryStream>;
}
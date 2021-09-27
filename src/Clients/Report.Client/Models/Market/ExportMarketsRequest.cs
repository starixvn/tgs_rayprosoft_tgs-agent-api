using System;

namespace TGS.Clients.Report.Client.Models.Market
{
    public record ExportMarketsRequest(Guid BrandId, DateTimeOffset From, DateTimeOffset To, string Culture);
}
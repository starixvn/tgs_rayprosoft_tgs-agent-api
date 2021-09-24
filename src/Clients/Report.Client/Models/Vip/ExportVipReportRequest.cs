using System;

namespace TGS.Clients.Report.Client.Models.Vip
{
    public record ExportVipReportRequest(Guid BrandId, string Username, int? Level, DateTimeOffset From,
        DateTimeOffset To)
    {
        public string AcceptLanguage { get; set; }
    }
}
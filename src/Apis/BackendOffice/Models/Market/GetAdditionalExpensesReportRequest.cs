using System;

namespace TGS.Apis.BackendOffice.Models.Market
{
    public record GetAdditionalExpensesReportRequest
    {
        public Guid BrandId { get; set; }

        public DateTimeOffset ReportDate { get; set; }
    }
}
using System;

namespace TGS.Clients.Report.Client.Models.Market
{
    public class GetAdditionalExpensesRequest
    {
        public Guid BrandId { get; set; }

        public DateTimeOffset ReportDate { get; set; }
    }
}
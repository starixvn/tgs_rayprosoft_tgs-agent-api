namespace TGS.Services.BackOffice.Application.Models.ReportVip
{
    public class GetVipReportReportResponse
    {
        public int TotalCount { get; init; }

        public VipReportModel[] Items { get; init; }

        public VipReportTotalModel Total { get; init; }
    }
}
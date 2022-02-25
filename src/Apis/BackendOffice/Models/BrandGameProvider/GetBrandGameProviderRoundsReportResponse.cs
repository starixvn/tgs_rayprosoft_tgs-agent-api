using TGS.Core.Models.Messages;

namespace TGS.Apis.BackendOffice.Models.BrandGameProvider
{
    public record GetBrandGameProviderRoundsReportResponse
    {
        public PagedResponse<BrandGameProviderRoundsReportModel> Datas { get; init; } = default!;

        public BrandGameProviderRoundTotalReportModel[] Totals { get; init; } = default!;
    }
}
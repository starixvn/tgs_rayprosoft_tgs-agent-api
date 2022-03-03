using TGS.Core.Models.Messages;

namespace TGS.Services.BackOffice.Application.Models.BrandGameProvider
{
    public record GetBrandGameProviderRoundsReportResponse
    {
        public PagedResponse<BrandGameProviderRoundsReportModel> Datas { get; init; } = default!;

        public BrandGameProviderRoundTotalReportModel[] Totals { get; init; } = default!;
    }
}
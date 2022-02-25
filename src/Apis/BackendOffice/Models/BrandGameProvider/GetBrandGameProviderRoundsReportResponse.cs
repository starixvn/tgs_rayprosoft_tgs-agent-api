using TGS.Core.Models.Messages;

namespace TGS.Apis.BackendOffice.Models.BrandGameProvider
{
    public record GetGameProviderRoundsReportResponse
    {
        public PagedResponse<BrandGameProviderRoundsReportModel> Datas { get; init; } = default!;

        public GameProviderRoundTotalReportModel[] Totals { get; init; } = default!;
    }
}
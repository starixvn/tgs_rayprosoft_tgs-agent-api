using TGS.Core.Models.Messages;

namespace TGS.Apis.BackendOffice.Models.GameProvider
{
    public record GetGameProviderRoundsReportResponse
    {
        public PagedResponse<GameProviderRoundsReportModel> Datas { get; init; } = default!;

        public GameProviderRoundTotalReportModel[] Totals { get; init; } = default!;
    }
}
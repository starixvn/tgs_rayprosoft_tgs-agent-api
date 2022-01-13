using TGS.Core.Models.Messages;

namespace TGS.Services.Report.Application.Models.GameProvider
{
    public class GetGameProviderRoundsResponse
    {
        public PagedResponse<GameProviderRoundsModel> Datas { get; set; }

        public GameProviderRoundTotalModel[] Totals { get; set; }
    }
}
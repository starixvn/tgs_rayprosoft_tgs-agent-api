using TGS.Core.Models.Messages;

namespace TGS.Services.BackOffice.Application.Models.GameProvider
{
    public class GetGameProviderBetLimitsResponse : MessageResponse
    {
        public GameProviderBetLimitModel[] BetLimits { get; set; }
    }
}
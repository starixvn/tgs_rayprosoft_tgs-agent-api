using MediatR;
using TGS.Core.Models.Messages;

namespace TGS.Services.Report.Application.Models.Market
{
    public class CreateMarketRequest : MarketRequest, IRequest<MessageResponse>
    {
    }
}
using MediatR;

namespace TGS.Shared.Game.Models.SBOBET.Incoming
{
    public record GetBalanceRequest : BaseRequest, IRequest<GetBalanceResponse>
    {
    }
}

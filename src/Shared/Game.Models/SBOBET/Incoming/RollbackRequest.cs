using MediatR;

namespace TGS.Shared.Game.Models.SBOBET.Incoming
{
    public record RollbackRequest : TransactionRequest, IRequest<RollbackResponse>
    {
    }
}
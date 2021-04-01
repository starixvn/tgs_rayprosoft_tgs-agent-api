using MediatR;
using TGS.Core.Models.Messages;

namespace Report.Client.Models.Aggregate
{
    public record CreateUpdatePlayerAggregateModel : IRequest<MessageResponse>;
}
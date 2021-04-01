using MediatR;
using TGS.Core.Models.Messages;

namespace Report.Client.Models.Aggregate
{
    public record ManualCreateUpdateRoundAggregateModel(string Date, int Hour) : IRequest<MessageResponse>;
}
using MediatR;
using TGS.Core.Models.Messages;

namespace Report.Client.Models.Aggregate
{
    public record ManualCreateUpdatePlayerAggregateModel(string Date, int Hour) : IRequest<MessageResponse>;
}
using MediatR;
using TGS.Core.Models.Messages;

namespace TGS.Services.Report.Application.Models.Aggregate
{
    public record ManualCreateUpdateRoundAggregateModel(string Date, int Hour) : IRequest<MessageResponse>;
}
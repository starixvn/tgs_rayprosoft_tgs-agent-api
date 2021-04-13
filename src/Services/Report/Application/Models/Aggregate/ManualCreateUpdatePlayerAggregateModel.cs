using MediatR;
using TGS.Core.Models.Messages;

namespace TGS.Services.Report.Application.Models.Aggregate
{
    public record ManualCreateUpdatePlayerAggregateModel(string Date, int Hour) : IRequest<MessageResponse>;
}
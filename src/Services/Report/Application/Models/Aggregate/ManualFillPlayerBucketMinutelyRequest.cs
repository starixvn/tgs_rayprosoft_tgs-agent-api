using MediatR;
using TGS.Core.Models.Messages;

namespace TGS.Services.Report.Application.Models.Aggregate
{
    public record ManualFillPlayerBucketMinutelyRequest
        (string Date, int Hour, int FromMinute, int ToMinute, bool FillPlayerBucket) : IRequest<MessageResponse>;
}
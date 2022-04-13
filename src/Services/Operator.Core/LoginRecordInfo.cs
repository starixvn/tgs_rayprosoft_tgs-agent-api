using System;
using TGS.Core.Models.Enums;

namespace TGS.Services.Operator
{
    public record LoginRecordInfo(
        Guid Id,
        PlatformType PlatformType,
        DateTimeOffset CreatedOn,
        DateTimeOffset ExpiredOn);
}

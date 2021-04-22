using System;
using System.Collections.Generic;
using TGS.Core.Models.Messages;

namespace TGS.Services.BackOffice.Application.Queries.Deposit
{
    public record DepositFormQuery(
        Guid BrandId,
        string UserName,
        string ChannelName,
        Guid? PaymentMethodId,
        IEnumerable<int> Status,
        (DateTimeOffset Begin, DateTimeOffset End)? CreateOnRange,
        (DateTimeOffset Begin, DateTimeOffset End)? UpdateOnRange,
        PageableRequest Paging);
}

using System;
using System.Collections.Generic;
using Tgs.Payment;
using TGS.Core.Models.Messages;

namespace TGS.Services.BackOffice.Application.Queries.Withdraw
{
    public record WithdrawFormQuery(
        Guid BrandId,
        string UserName,
        string ChannelName,
        Guid? PaymentMethodId,
        IEnumerable<PaymentFlowState> Status,
        (DateTimeOffset Begin, DateTimeOffset End)? CreateOnRange,
        (DateTimeOffset Begin, DateTimeOffset End)? UpdateOnRange,
        PageableRequest Paging);
}

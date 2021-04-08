using System;

namespace TGS.Services.BackOffice.Application.Queries.Deposit
{
    public record DepositFormQuery(
        Guid BrandId,
        string UserName,
        string ChannelName,
        Guid? PaymentMethodId,
        int? Status,
        (DateTimeOffset Begin, DateTimeOffset End)? CreateOnRange,
        (DateTimeOffset Begin, DateTimeOffset End)? UpdateOnRange);
}

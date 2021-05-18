using System;

namespace TGS.Services.Payment
{
    public record BrandPaymentInfo(
        Guid Id,
        Guid PaymentMethodChannelId,
        Guid PaymentId,
        Guid MethodId,
        Guid ChannelId,
        string ChannelName,
        string ChannelCode,
        string ExtendedData,
        ValueRange<decimal> TransferLimit,
        bool IsAllPlayers,
        bool AuthenticateIdentity,
        bool AuthenticatePhone,
        ValueRange<decimal> DepositedAmountLimit,
        ConditionMatchTypes ConditionMatchType,
        TransferType TransferType,
        bool Enabled);
}

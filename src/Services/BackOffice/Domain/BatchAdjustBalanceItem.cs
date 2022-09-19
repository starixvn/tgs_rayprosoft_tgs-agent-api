#nullable enable

using System;

namespace TGS.Services.BackOffice.Domain;
public record BatchAdjustBalanceItem(
    Guid WalletId,
    string PlayerCode,
    string UserName,
    decimal AdjustAmount,
    decimal Turnover,
    string? Remark);

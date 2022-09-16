#nullable enable

namespace TGS.Services.BackOffice.Domain;
public record BatchAdjustBalanceItem(
    string UserName,
    decimal AdjustAmount,
    decimal Turnover,
    string? Remark);

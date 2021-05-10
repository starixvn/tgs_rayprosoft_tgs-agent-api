using TGS.Core.Models.Enums;

namespace TGS.Apis.TgsApi.GameProviders.Saba.Data
{
    public record SettleTransaction(
            string PlayerCode,
            string ExternalTransactionId,
            string UpdateTime,
            string WinlostDate,
            GameActionType GameActionType,
            decimal Amount)
        : TransactionInfo(PlayerCode, ExternalTransactionId, UpdateTime);
}
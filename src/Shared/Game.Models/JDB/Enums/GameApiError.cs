// unset

namespace TGS.Shared.Game.Models.JDB.Enums
{
    public enum GameApiError
    {
        Succeed = 0,
        Failed = 9999,
        DuplicateTransactions = 9011,
        SystemIsMaintained = 9013,
        DataDoesNotExist = 9015,
        InvalidParameters = 8000,
        UserIdCannotBeFound = 7501,
        BalanceNotEnough = 6006,
    }

}
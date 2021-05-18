// unset

using System.ComponentModel;

namespace TGS.Shared.Game.Models.JDB.Enums
{
    public enum GameApiStatus
    {
        [Description("0000")]
        Succeed = 0,
        [Description("9999")]
        Failed = 9999,
        [Description("9011")]
        DuplicateTransactions = 9011,
        [Description("9013")]
        SystemIsMaintained = 9013,
        [Description("9015")]
        DataDoesNotExist = 9015,
        [Description("8000")]
        InvalidParameters = 8000,
        [Description("7501")]
        UserIdCannotBeFound = 7501,
        [Description("6006")]
        BalanceNotEnough = 6006
    }

}
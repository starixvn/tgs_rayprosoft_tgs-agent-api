// unset

namespace TGS.Shared.Game.Models.JDB.Outgoing
{
    public static class ErrorCodes
    {
        public const string Succeed = "0000";
        public const string Failed = "9999";
        public const string DuplicateTransactions = "9011";
        public const string SystemIsMaintained = "9013";
        public const string DataDoesNotExist = "9015";
        public const string InvalidParameters = "8000";
        public const string UserIdCannotBeFound = "7501";
        public const string BalanceNotEnough = "6006";
    }
}
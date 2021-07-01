namespace TGS.Shared.Game.Models.AllBet.Enums
{
    public enum GameApiError
    {
        Success = 0,
        InvalidOperatorId = 10000,
        InvalidSignature = 10001,
        UserIdNotFound = 10003,
        LoginProhibited = 10005,
        TransactionIsNotFound = 10006,
        InvalidGameActionOperation = 10007,
        PlaceBetProhibited = 10100,
        InsufficientFunds = 10101,
        UnderMaintenance = 10200,
        InvalidArgument = 40000,
        SystemError = 50000
    }
}
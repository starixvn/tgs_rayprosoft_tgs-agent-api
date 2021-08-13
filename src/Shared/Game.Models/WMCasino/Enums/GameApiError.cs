namespace TGS.Shared.Game.Models.WMCasino.Enums
{
    public enum GameApiError
    {
        Success = 0,
        InvalidVendorIdOrSignature = 103,
        InvalidCredential = 101,
        InvalidTimeFormat = 102,
        PlayerCodeExist = 104,
        NoResult = 107,
        InvalidParameter = 900,
        Maintenance = 911,
        ReportAllowedADayOnly = 10201,
        EmptyVendorId = 10301,
        InvalidVendorId = 10302,
        InvalidSignature = 10303,
        EmptySignature = 10304,
        PlayerCodeTooLong = 10404,
        PlayerCodeTooShort = 10405,
        UsernameTooLong = 10409,
        PlayerHasPendingTransaction = 10410,
        TryAgain = 10411,
        InvalidPlayerCode = 10501,
        EmptyPlayerCode = 10502,
        SuspendPlayer = 10505,
        EmptyUsername = 10509,
        InsufficientFunds = 10805,


        GameIsNotFound = 99996,
        RoundIsNotFound = 99997,
        TransactionIsNotFound = 99998,
        SystemError = 99999
    }
}
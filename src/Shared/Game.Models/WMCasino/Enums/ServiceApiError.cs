namespace TGS.Shared.Game.Models.WMCasino.Enums
{
    public enum ServiceApiError
    {
        Success = 0,
        ServerError = 99999,
        InvalidVendorIdOrSignature = 103,
        InvalidCredential = 101,
        InvalidTimeFormat = 102,
        UserExist = 104,
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
        UserNotExist = 10501,
        EmptyPlayerCode = 10502,
        SuspendPlayer = 10505,
        EmptyPassword = 10508,
        EmptyUsername = 10509,
        InsufficientFund = 10805
    }
}
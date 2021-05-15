namespace TGS.Shared.Game.Models.Saba.Enums
{
    public enum GameApiError
    {
        Undefined = -1,
        Success = 0,
        DuplicateTransaction = 1,
        IncorrectParameter = 101,
        AccountIsClosed = 201,
        AccountIsLocked = 202,
        AccountIsNotExist = 203,
        OperatorIdNotExist = 301,
        InvalidCurrency = 302,
        InvalidUserId = 303,
        LanguageIsNotSupport = 304,
        InvalidToken = 305,
        InvalidTimezone = 306,
        InvalidAmount = 307,
        InvalidDateTimeFormat = 308,
        InvalidTransactionStatus = 309,
        InvalidBetLimitSetting = 310,
        InvalidAuthenticationKey = 311,
        InvalidIpAddress = 312,
        PermissionDenied = 501,
        PlayerHasInsufficientFunds = 502,
        IpAddressRestricted = 503,
        NoTicket = 504,
        NoData = 505,
        UnableProcessed = 506,
        BetNotEligibleForBonus = 507,
        DatabaseError = 901,
        NetworkError = 902,
        SystemIsUnderMaintenance = 903,
        RequestTimeout = 904,
        SystemBusy = 905,
        SystemError = 999
    }
}